using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Accessories.NeutralItems;
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
using Terraria.WorldBuilding;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class VindicatorsAxe : NeutralItem
    {
        public const int ArmorPenetration = 8;
        public const float SilencedDamageBoost = 0.15f;
        public const int SilenceDuration = 15 * 60;
        public const int SilenceCooldown = 5 * 60;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ArmorPenetration, SilencedDamageBoost.ToPercent(), SilenceDuration.FramesToSeconds());
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 38;
            Item.height = 32;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = MogGlobalItem.RarityLightPurpleBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingVindicatorsAxe = true;
            player.GetArmorPenetration(DamageClass.Generic) += ArmorPenetration;
        }
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            base.ModifyTooltips(list);
            list.IntegrateHotkey(KeybindSystem.NeutralItemKeybind);
        }
        ModKeybind keybindActive = null;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MoltenHamaxe).
                AddIngredient(ItemID.DarkShard).
                AddIngredient<ScorchedCore>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
namespace MogMod.Common.MogModPlayer
{
    public partial class MogPlayer : ModPlayer
    {
        public void VindicatorsAxeEffect()
        {
            if (KeybindSystem.NeutralItemKeybind.JustPressed && wearingVindicatorsAxe && vindicatorsAxeCooldown <= 0 && !Player.HasBuff(BuffID.Silenced))
            {
                vindicatorsAxeCooldown = VindicatorsAxe.SilenceCooldown;
                VindicatorsAxeDustEffect(Player);
                if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    VindicatorsAxeSync(Player, false);
                }
            }
        }
        public void VindicatorsAxeBuff()
        {
            if (wearingVindicatorsAxe)
            {
                if (vindicatorsAxeCooldown > 0) vindicatorsAxeCooldown--;
                if (Player.HasBuff(BuffID.Silenced))
                    Player.GetDamage<GenericDamageClass>() += VindicatorsAxe.SilencedDamageBoost;
            }
        }
        public static void VindicatorsAxeDustEffect(Player player)
        {
            player.AddBuff(BuffID.Silenced, (int)(VindicatorsAxe.SilenceDuration / (Main.masterMode ? 2.5f : Main.expertMode ? 2f : 1f))); // will move to vindAxeEffect
            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, player.Center);
            for (int i = 0; i < 150; i++)
            {
                Vector2 circlePos = Main.rand.NextVector2CircularEdge(player.width * 3f, player.height * 0.75f);
                Vector2 circleVel = player.DirectionFrom(player.Center + player.velocity + circlePos) * Main.rand.NextFloat(5f, 7f);
                Dust axeCircle = Dust.NewDustPerfect(player.Center + circlePos, DustID.LavaMoss, circleVel, 100);
                axeCircle.fadeIn = 1.45f;
                axeCircle.noGravity = true;
            }
        }
        public void VindicatorsAxeSync(Player player, bool server)
        {
            ModPacket packet = Mod.GetPacket(256);

            packet.Write((byte)MogModMessageType.VindicatorsAxeSync);
            packet.Write(player.whoAmI);

            player.SendPacket(packet, server);
        }
        internal void HandleVindicatorsAxe(BinaryReader reader)
        {
            Player client = reader.ReadPlayer();
            if (Main.netMode == NetmodeID.Server)
            {
                VindicatorsAxeSync(client, true);
            }
            VindicatorsAxeDustEffect(client);
        }
    }
}