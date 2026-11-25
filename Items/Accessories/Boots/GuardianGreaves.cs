using Microsoft.Xna.Framework;
using MogMod.Buffs.AccessoryAuras;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Accessories.Boots;
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

namespace MogMod.Items.Accessories.Boots
{
    public class GuardianGreaves : ModItem, ILocalizedModType
    {
        // auras
        public const int LifeBoost = 20;
        public const int AuraManaBoost = 50;
        public const int LifeRegenBoost = 8;
        public const int DefenseBoost = 8;
        public const float MagicDamageBoost = 0.08f;
        // accessory buffs
        public const int ManaBoost = 50;
        // use effect
        public const int LifeHeal = 140;
        public const int ManaHeal = 300;
        public const int ManaAndLifeHealCooldown = 60 * 60;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaBoost, LifeBoost, AuraManaBoost, LifeRegenBoost.ToRegenPerSecond(), DefenseBoost, MagicDamageBoost.ToPercent(), LifeHeal, ManaHeal, ManaAndLifeHealCooldown.FramesToSeconds());
        int teamBuff = ModContent.BuffType<GuardianGreavesAura>();
        public new string LocalizationCategory => "Items.Accessories.Boots";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.BootsKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // give mana boots an on button press affect that restores 200 mana and if possible does so to everyone
            player.accRunSpeed = 7.5f;
            player.moveSpeed += .25f;
            player.statManaMax2 += ManaBoost;
            player.tileSpeed += .40f;
            player.aggro -= 1000;
            player.manaRegen += (int)Math.Round(player.manaRegen * .5f);
            player.manaRegenDelay -= 4f;
            Player.tileRangeX = Player.tileRangeY += 3;
            // a check on whether the player is wearing boots
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingGuardianGreaves = true;

            //provides a buff to players on your team
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
            player.rocketBoots = player.vanityRocketBoots = 3;
            player.waterWalk2 = true; // Allows walking on all liquids without falling into it
            player.waterWalk = true;
            player.iceSkate = true; // Grant the player improved speed on ice and not breaking thin ice when falling onto it
            player.fireWalk = true; // Grants the player immunity from Meteorite and Hellstone tile damage
            player.lavaRose = true; // Grants the Lava Rose effect
            player.lavaMax += 240; // Grants the player 2 additional seconds of lava immunity

            if (!hideVisual)
            {
                player.CancelAllBootRunVisualEffects();
                player.coldDash = true;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ArcaneBoots>().
                AddIngredient<Mekansm>().
                AddIngredient(ItemID.TerrasparkBoots).
                AddIngredient(ItemID.SoulofMight, 7).
                AddRecipeGroup("AnyCobaltBar", 5).
                AddIngredient<FrigidCrystal>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
namespace MogMod.Common.MogModPlayer
{
    public partial class MogPlayer : ModPlayer
    {
        public void GuardianGreavesEffect()
        {
            if (KeybindSystem.BootsKeybind.JustPressed && wearingGuardianGreaves && !Player.HasBuff(GuardianGreavesCooldown))
            {
                GuardianGreavesBuff(Player, true);
                GuardianGreavesDustEffect(Player);
                if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    GuardianGreavesSync(Player, false);
                }
            }
        }
        public static void GuardianGreavesBuff(Player player, bool server)
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
                            client.HealLifeMult(GuardianGreaves.LifeHeal);
                            client.HealManaMult(GuardianGreaves.ManaHeal);
                            client.MogMod().mekansmAndGreavesLifeCooldown = GuardianGreaves.ManaAndLifeHealCooldown;
                        }
                        GuardianGreavesDustEffect(client);
                    }
                }
            }
            if (!server) return;
            if (mogPlayer.mekansmAndGreavesLifeCooldown <= 0) player.HealLifeMult(GuardianGreaves.LifeHeal);
            player.HealManaMult(GuardianGreaves.ManaHeal);
            mogPlayer.mekansmAndGreavesLifeCooldown = GuardianGreaves.ManaAndLifeHealCooldown;
            player.AddBuff(mogPlayer.GuardianGreavesCooldown, GuardianGreaves.ManaAndLifeHealCooldown);
        }
        public static void GuardianGreavesDustEffect(Player player)
        {
            SoundEngine.PlaySound(GuardianGreavesActivateSound, player.Center);
            for (int i = 0; i < 100; i++)
            {
                Vector2 shinePos = Main.rand.NextVector2Circular(player.width * 0.75f, player.height * 0.5f);
                Vector2 shineVel = Vector2.One * Main.rand.NextFloat(-3.25f, 3.26f);
                Dust greavesPop = Dust.NewDustPerfect(player.Center + shinePos, DustID.UltraBrightTorch, shineVel, 100);
                greavesPop.fadeIn = 1.5f;
                greavesPop.velocity *= 0.85f;
                greavesPop.noGravity = true;
            }
        }
        public void GuardianGreavesSync(Player player, bool server)
        {
            ModPacket packet = Mod.GetPacket(256);

            packet.Write((byte)MogModMessageType.GuardianGreavesSync);
            packet.Write(player.whoAmI);

            player.SendPacket(packet, server);
        }
        internal void HandleGuardianGreaves(BinaryReader reader)
        {
            Player client = reader.ReadPlayer();
            if (Main.netMode == NetmodeID.Server)
            {
                GuardianGreavesSync(client, true);
            }
            GuardianGreavesBuff(client, false);
            GuardianGreavesDustEffect(client);
        }
    }
}