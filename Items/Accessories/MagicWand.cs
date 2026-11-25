using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Accessories;
using MogMod.Items.Global;
using MogMod.Items.Other;
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
    public class MagicWand : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int FlatMagicDamageBoost = 3;
        public const int ManaBoost = 30;
        public const int LifeHeal = 7; // shared with mana
        public const int ManaHeal = 7; // shared with life
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FlatMagicDamageBoost, ManaBoost, LifeHeal);
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.WandKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = Item.height = 28;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingMagicWand = true;
            player.GetDamage(DamageClass.Magic).Flat += FlatMagicDamageBoost;
            player.statManaMax2 += ManaBoost;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MagicStick>().
                AddIngredient<IronBranch>(2).
                AddIngredient<CraftingRecipe>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
namespace MogMod.Common.MogModPlayer
{
    public partial class MogPlayer : ModPlayer
    {
        public void MagicWandEffect()
        {
            if (KeybindSystem.WandKeybind.JustPressed && wearingMagicWand && wandCharges > 0)
            {
                MagicWandBuff(false);
                MagicWandDustEffect(Player);
                if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    MagicWandSync(Player, false);
                }
            }
        }
        public void MagicWandBuff(bool hit)
        {
            if (hit)
            {
                if (wearingMagicWand)
                {
                    wandCharges++;
                    if (wandCharges > maxWandCharges)
                    {
                        wandCharges = maxWandCharges;
                    }
                }
            }
            else
            {
                int heal = MagicWand.LifeHeal * wandCharges;
                int mana = MagicWand.ManaHeal * wandCharges;

                Player.HealLifeMult(heal);
                Player.HealManaMult(mana);

                wandCharges = 0;
                SoundEngine.PlaySound(WandUse, Player.Center);
            }
        }
        public static void MagicWandDustEffect(Player player)
        {
            SoundEngine.PlaySound(WandUse, player.Center);
            for (int i = 0; i < 50; i++)
            {
                Vector2 shinePos = Main.rand.NextVector2CircularEdge(player.width, player.height * 0.5f);
                Vector2 newShinePos = new(shinePos.X, -MathF.Abs(shinePos.Y));
                Vector2 shineVel = Vector2.UnitY * -Main.rand.NextFloat(-0.5f, 3.51f);
                Dust wandHeal = Dust.NewDustPerfect(player.Center + newShinePos, DustID.AncientLight, shineVel, 100, Color.Aquamarine);
                wandHeal.fadeIn = 1.45f;
                wandHeal.velocity *= 0.98f;
                wandHeal.scale *= 0.85f;
                wandHeal.noGravity = true;
            }
        }
        public void MagicWandSync(Player player, bool server)
        {
            ModPacket packet = Mod.GetPacket(256);

            packet.Write((byte)MogModMessageType.MagicWandSync);
            packet.Write(player.whoAmI);

            player.SendPacket(packet, server);
        }
        internal void HandleMagicWand(BinaryReader reader)
        {
            Player client = reader.ReadPlayer();
            if (Main.netMode == NetmodeID.Server)
            {
                MagicWandSync(client, true);
            }
            MagicWandDustEffect(client);
        }
    }
}