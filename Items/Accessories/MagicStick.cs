using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Accessories;
using MogMod.Items.Global;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.Items.Accessories
{
    public class MagicStick : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int LifeHeal = 5; // shared with mana
        public const int ManaHeal = 5; // shared with life
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifeHeal);
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.WandKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 20;
            Item.height = 26;
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingMagicStick = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup(RecipeGroupID.Wood, 25).
                AddIngredient(ItemID.Emerald).
                AddIngredient(ItemID.FallenStar).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
namespace MogMod.Common.MogModPlayer
{
    public partial class MogPlayer : ModPlayer
    {
        public void MagicStickEffect()
        {
            if (KeybindSystem.WandKeybind.JustPressed && wearingMagicStick && stickCharges > 0)
            {
                MagicStickBuff(false);
                MagicStickDustEffect(Player);
                if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    MagicStickSync(Player, false);
                }
            }
        }
        public void MagicStickBuff(bool hit)
        {
            if (hit)
            {
                if (wearingMagicStick)
                {
                    stickCharges++;
                    if (stickCharges > maxStickCharges)
                    {
                        stickCharges = maxStickCharges;
                    }
                }
            }
            else
            {
                int heal = MagicStick.LifeHeal * stickCharges;
                int mana = MagicStick.ManaHeal * stickCharges;

                Player.HealLifeMult(heal);
                Player.HealManaMult(mana);

                stickCharges = 0;
                SoundEngine.PlaySound(WandUse, Player.Center);
            }
        }
        public static void MagicStickDustEffect(Player player)
        {
            SoundEngine.PlaySound(WandUse, player.Center);
            for (int i = 0; i < 25; i++)
            {
                Vector2 shinePos = Main.rand.NextVector2CircularEdge(player.width * 0.95f, player.height * 0.475f);
                Vector2 newShinePos = new(shinePos.X, -MathF.Abs(shinePos.Y));
                Vector2 shineVel = Vector2.UnitY * -Main.rand.NextFloat(-0.75f, 1.51f);
                Dust stickHeal = Dust.NewDustPerfect(player.Center + newShinePos, DustID.AncientLight, shineVel, 100, Color.LimeGreen);
                stickHeal.fadeIn = 1.45f;
                stickHeal.velocity *= 0.98f;
                stickHeal.scale *= 0.85f;
                stickHeal.noGravity = true;
            }
        }
        public void MagicStickSync(Player player, bool server)
        {
            ModPacket packet = Mod.GetPacket(256);

            packet.Write((byte)MogModMessageType.MagicStickSync);
            packet.Write(player.whoAmI);

            player.SendPacket(packet, server);
        }
        internal void HandleMagicStick(BinaryReader reader)
        {
            Player client = reader.ReadPlayer();
            if (Main.netMode == NetmodeID.Server)
            {
                MagicStickSync(client, true);
            }
            MagicStickDustEffect(client);
        }
    }
}