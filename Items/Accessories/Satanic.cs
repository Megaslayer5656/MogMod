using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Accessories;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Items.Weapons.Melee;
using MogMod.Utilities;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.Items.Accessories
{
    public class Satanic : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const float DamageBoost = 0.1f;
        public const int MaxLifeBoost = 50;
        public const int SatanicDuration = 8 * 60;
        public const int SatanicCooldown = 120 * 60;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 50;
            // makes vampire knives crazy
            player.lifeSteal *= 2f;
            player.GetDamage(DamageClass.Generic) += DamageBoost;
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingSatanic = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var Hotkey = KeybindSystem.SatanicKeybind.TooltipHotkeyString();
            int index = tooltips.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (index != -1)
            {
                if (Main.zenithWorld)
                {
                    index++;
                    TooltipLine gfb = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<Satanic>("TooltipGFB").Format());
                    tooltips.Insert(index, gfb);
                }
                else
                {
                    index++;
                    TooltipLine normal = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<Satanic>("TooltipNormal").Format(
                    MaxLifeBoost,
                    DamageBoost.ToPercent(),
                    Hotkey,
                    SatanicDuration.FramesToSeconds()));
                    tooltips.Insert(index, normal);
                }
            }
        }
        ModKeybind keybindActive = null;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Sange>().
                AddIngredient(ItemID.MoonStone).
                AddIngredient(ItemID.BeetleHusk, 7).
                AddIngredient<GriefBar>(5).
                AddIngredient<VitalityBooster>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
namespace MogMod.Common.MogModPlayer
{
    public partial class MogPlayer : ModPlayer
    {
        public void SatanicEffect()
        {
            if (KeybindSystem.SatanicKeybind.JustPressed && wearingSatanic && !Player.HasBuff(SatanicCooldown))
            {
                Player.AddBuff(SatanicBuffID, Satanic.SatanicDuration);
                Player.AddBuff(SatanicCooldown, Satanic.SatanicCooldown);
                SatanicDustEffect(Player);
                if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    SatanicSync(Player, false);
                }
            }
        }
        public void SatanicBuff(bool GFB)
        {
            if (wearingSatanic)
            {
                if (GFB)
                {
                    if (Main.zenithWorld)
                        DamageOverTime(50);
                }
                if (satanicBuff)
                {
                    Player.lifeSteal *= 4;
                    Player.blind = true;
                    Player.blackout = true; // greatly reduced light effect
                    Player.headcovered = true; // nebula headcrab effect
                }
            }
        }
        public static void SatanicDustEffect(Player player)
        {
            SoundEngine.PlaySound(SatanicActivateSound, player.Center);
            for (int i = 0; i < 150; i++)
            {
                Vector2 circlePos = Main.rand.NextVector2CircularEdge(player.width * 3f, player.height * 0.75f);
                Vector2 circleVel = player.DirectionFrom(player.Center + player.velocity + circlePos) * Main.rand.NextFloat(1f, 3f);
                Dust satanicEye = Dust.NewDustPerfect(player.Center + circlePos * Main.rand.NextFloat(0.75f, 1.25f), ChildSafety.Disabled ? DustID.Blood : DustID.CrimsonPlants, circleVel, 100);
                satanicEye.fadeIn = 1.45f;
                satanicEye.noGravity = true;
            }
            for (int i = 0; i < 75; i++)
            {
                Vector2 circlePos = Main.rand.NextVector2Circular(player.width * 0.25f, player.height * 1.35f);
                Vector2 circleVel = player.DirectionFrom(player.Center + player.velocity + circlePos) * Main.rand.NextFloat(2f, 4f);
                Dust satanicPupil = Dust.NewDustPerfect(player.Center + circlePos, DustID.CrystalPulse2, circleVel, 100);
                satanicPupil.fadeIn = 1.85f;
                satanicPupil.noGravity = true;
            }
        }
        public void SatanicSync(Player player, bool server)
        {
            ModPacket packet = Mod.GetPacket(256);

            packet.Write((byte)MogModMessageType.SatanicSync);
            packet.Write(player.whoAmI);

            player.SendPacket(packet, server);
        }
        internal void HandleSatanic(BinaryReader reader)
        {
            Player client = reader.ReadPlayer();
            if (Main.netMode == NetmodeID.Server)
            {
                SatanicSync(client, true);
            }
            SatanicDustEffect(client);
        }
    }
}