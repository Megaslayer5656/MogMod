using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Accessories;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
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
    // TODO: this aswell as alot of other process trigger accessories should move their effects in their respective item files
    [AutoloadEquip(EquipType.Back, EquipType.Front)]
    public class GlimmerCape : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        // accessory buffs
        public const int ManaBoost = 50;
        public const int AggroBoost = 400;
        // effect buffs
        public const int GlimmerDuration = 30 * 60;
        public const int GlimmerCooldown = 60 * 60;
        public const float MovementSpeedBoost = 0.25f;
        public const int ManaRegenBoost = 4;
        public const int BuffAggroBoost = 750;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaBoost, AggroBoost);
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.GlimmerCapeKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = Item.height = 24;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statManaMax2 += ManaBoost;
            player.aggro -= AggroBoost;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingGlimmerCape = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Robe).
                AddIngredient<ShadowAmulet>().
                AddIngredient<FrigidShard>(3).
                AddIngredient<ManaEssence>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
namespace MogMod.Common.MogModPlayer
{
    public partial class MogPlayer : ModPlayer
    {
        public void GlimmerCapeEffect()
        {
            if (KeybindSystem.GlimmerCapeKeybind.JustPressed && wearingGlimmerCape && !Player.HasBuff(GlimmerCapeCooldown))
            {
                Player.AddBuff(GlimmerCapeBuffID, GlimmerCape.GlimmerDuration);
                Player.AddBuff(GlimmerCapeCooldown, GlimmerCape.GlimmerCooldown);
                GlimmerCapeDustEffect(Player);
                if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    GlimmerCapeSync(Player, false);
                }
            }
        }
        public void GlimmerCapeBuff()
        {
            if (glimmerActive)
            {
                Player.invis = true;
                Player.moveSpeed += GlimmerCape.MovementSpeedBoost;
                Player.manaRegenBonus += GlimmerCape.ManaRegenBoost;
                Player.manaRegenDelay -= 4f;
                Player.aggro -= GlimmerCape.BuffAggroBoost;
            }
        }
        public static void GlimmerCapeDustEffect(Player player)
        {
            SoundEngine.PlaySound(GlimmerActivateSound, player.Center);
            for (int i = 0; i < 50; i++)
            {
                Vector2 shinePos = Main.rand.NextVector2Circular(player.width * 1.15f, player.height * 0.35f);
                Vector2 shineVel = Vector2.UnitY * -Main.rand.NextFloat(-0.25f, 4.5f);
                Dust glimmerShine = Dust.NewDustPerfect(player.Center + shinePos, DustID.DungeonSpirit, shineVel, 100);
                glimmerShine.fadeIn = 1.45f;
                glimmerShine.velocity *= 0.85f;
                glimmerShine.noGravity = true;
            }
        }
        public void GlimmerCapeSync(Player player, bool server)
        {
            ModPacket packet = Mod.GetPacket(256);

            packet.Write((byte)MogModMessageType.GlimmerCapeSync);
            packet.Write(player.whoAmI);

            player.SendPacket(packet, server);
        }
        internal void HandleGlimmerCape(BinaryReader reader)
        {
            Player client = reader.ReadPlayer();
            if (Main.netMode == NetmodeID.Server)
            {
                GlimmerCapeSync(client, true);
            }
            GlimmerCapeDustEffect(client);
        }
    }
}