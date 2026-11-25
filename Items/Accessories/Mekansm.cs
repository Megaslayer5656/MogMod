using Microsoft.Xna.Framework;
using MogMod.Buffs.AccessoryAuras;
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
    public class Mekansm : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int LifeRegenBoost = 4;
        public const int LifeHeal = 80;
        public const int LifeHealCooldown = 60 * 60;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifeRegenBoost.ToRegenPerSecond(), LifeHeal, LifeHealCooldown.FramesToSeconds());
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.MekansmKeybind);
        ModKeybind keybindActive = null;
        int teamBuff = ModContent.BuffType<HeaddressBuff>();
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
            Item.defense = 4;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.lifeRegen += LifeRegenBoost;
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingMekansm = true;
            if (player.miscCounter % 10 == 0)
            {
                int myPlayer = Main.myPlayer;
                player.AddBuff(teamBuff, 20);
                if (Main.player[myPlayer].team == player.team && player.team != 0)
                {
                    float teamPlayerXDist = player.position.X - Main.player[myPlayer].position.X;
                    float teamPlayerYDist = player.position.Y - Main.player[myPlayer].position.Y;
                    if ((float)Math.Sqrt(teamPlayerXDist * teamPlayerXDist + teamPlayerYDist * teamPlayerYDist) < mogPlayer.auraRange)
                    {
                        Main.player[myPlayer].AddBuff(teamBuff, 20);
                    }
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Headdress>().
                AddIngredient(ItemID.Bone, 20).
                AddRecipeGroup("AnySilverBar", 15).
                AddIngredient<FrigidShard>(5).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
namespace MogMod.Common.MogModPlayer
{
    public partial class MogPlayer : ModPlayer
    {
        public void MekansmEffect()
        {
            if (KeybindSystem.MekansmKeybind.JustPressed && wearingMekansm && !Player.HasBuff(MekansmCooldown))
            {
                MekansmBuff(Player, true);
                MekansmDustEffect(Player);
                if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    MekansmSync(Player, false);
                }
            }
        }
        public static void MekansmBuff(Player player, bool server)
        {
            MogPlayer mogPlayer = player.MogMod();
            for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
            {
                Player client = Main.player[playerIndex];

                float teamPlayerXDist = player.position.X - client.position.X;
                float teamPlayerYDist = player.position.Y - client.position.Y;
                if ((float)Math.Sqrt(teamPlayerXDist * teamPlayerXDist + teamPlayerYDist * teamPlayerYDist) < mogPlayer.auraRange)
                {
                    if (client.team == player.team && client.whoAmI != player.whoAmI && player.team != 0)
                    {
                        if (!server && client.MogMod().mekansmAndGreavesLifeCooldown <= 0)
                        {
                            client.HealLifeMult(Mekansm.LifeHeal);
                            client.MogMod().mekansmAndGreavesLifeCooldown = Mekansm.LifeHealCooldown;
                        }
                        MekansmDustEffect(client);
                    }
                }
            }
            if (!server) return;
            if (mogPlayer.mekansmAndGreavesLifeCooldown <= 0) player.HealLifeMult(Mekansm.LifeHeal);
            mogPlayer.mekansmAndGreavesLifeCooldown = Mekansm.LifeHealCooldown;
            player.AddBuff(mogPlayer.MekansmCooldown, Mekansm.LifeHealCooldown);
        }
        public static void MekansmDustEffect(Player player)
        {
            SoundEngine.PlaySound(MekansmActivateSound, player.Center);
            for (int i = 0; i < 50; i++)
            {
                Vector2 shinePos = Main.rand.NextVector2Circular(player.width * 0.85f, player.height * 0.25f);
                Vector2 shineVel = Vector2.UnitY * -Main.rand.NextFloat(-0.25f, 3f);
                Dust healOrb = Dust.NewDustPerfect(player.Center + shinePos, DustID.JungleTorch, shineVel, 100);
                healOrb.fadeIn = 1.5f;
                healOrb.velocity *= 0.98f;
                healOrb.noGravity = true;
            }
        }
        public void MekansmSync(Player player, bool server)
        {
            ModPacket packet = Mod.GetPacket(256);

            packet.Write((byte)MogModMessageType.MekansmSync);
            packet.Write(player.whoAmI);

            player.SendPacket(packet, server);
        }
        internal void HandleMekansm(BinaryReader reader)
        {
            Player client = reader.ReadPlayer();
            if (Main.netMode == NetmodeID.Server)
            {
                MekansmSync(client, true);
            }
            MekansmBuff(client, false);
            MekansmDustEffect(client);
        }
    }
}