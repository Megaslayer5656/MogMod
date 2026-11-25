using Microsoft.Xna.Framework;
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
    [AutoloadEquip(EquipType.Shoes)]
    public class ArcaneBoots : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories.Boots";
        public const int MaxManaBoost = 50;
        public const int ManaHeal = 200;
        public const int ManaHealCooldown = 60 * 60;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxManaBoost, ManaHeal);
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.BootsKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 26;
            Item.height = 28;
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingArcaneBoots = true;
            player.accRunSpeed = 6.5f;
            player.moveSpeed += 0.12f;
            player.statManaMax2 += MaxManaBoost;
            player.manaRegen += (int)Math.Round(player.manaRegen * 0.2f);
            player.rocketBoots = 2;
            player.vanityRocketBoots = 2;
            if (!hideVisual)
            {
                player.CancelAllBootRunVisualEffects();
                player.coldDash = true;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpectreBoots).
                AddIngredient(ItemID.BandofStarpower).
                AddIngredient<ManaEssence>(2).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
namespace MogMod.Common.MogModPlayer
{
    public partial class MogPlayer : ModPlayer
    {
        public void ArcaneBootsEffect()
        {
            if (KeybindSystem.BootsKeybind.JustPressed && wearingArcaneBoots && !Player.HasBuff(ArcaneBootsCooldown))
            {
                ArcaneBootsBuff(Player, true);
                ArcaneBootsDustEffect(Player);
                if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ArcaneBootsSync(Player, false);
                }
            }
        }
        public static void ArcaneBootsBuff(Player player, bool server)
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
                        if (!server) client.HealManaMult(ArcaneBoots.ManaHeal);
                        ArcaneBootsDustEffect(client);
                    }
                }
            }
            if (!server) return;
            player.HealManaMult(ArcaneBoots.ManaHeal);
            player.AddBuff(mogPlayer.ArcaneBootsCooldown, ArcaneBoots.ManaHealCooldown);
        }
        public static void ArcaneBootsDustEffect(Player player)
        {
            SoundEngine.PlaySound(ArcaneBootsActivateSound, player.Center);
            for (int i = 0; i < 50; i++)
            {
                Vector2 shinePos = Main.rand.NextVector2Circular(player.width * 1f, player.height * 0.5f);
                Vector2 shineVel = Vector2.UnitY * -Main.rand.NextFloat(-0.25f, 3.25f);
                Dust manaBurst = Dust.NewDustPerfect(player.Center + shinePos, DustID.ManaRegeneration, shineVel, 100);
                manaBurst.fadeIn = 1.45f;
                manaBurst.velocity *= 0.85f;
                manaBurst.noGravity = true;
            }
        }
        public void ArcaneBootsSync(Player player, bool server)
        {
            ModPacket packet = Mod.GetPacket(256);

            packet.Write((byte)MogModMessageType.ArcaneBootsSync);
            packet.Write(player.whoAmI);

            player.SendPacket(packet, server);
        }
        internal void HandleArcaneBoots(BinaryReader reader)
        {
            Player client = reader.ReadPlayer();
            if (Main.netMode == NetmodeID.Server)
            {
                ArcaneBootsSync(client, true);
            }
            ArcaneBootsBuff(client, false);
            ArcaneBootsDustEffect(client);
        }
    }
}