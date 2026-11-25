using Microsoft.Xna.Framework;
using MogMod.Buffs.Cooldowns;
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
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.Items.Accessories
{
    public class RefresherOrb : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const float MagicAndSummonDamageBoost = 0.1f;
        public const int ManaBoost = 50;
        public const int RefresherCooldown = 180 * 60;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingRefresherOrb = true;
            player.GetDamage(DamageClass.Magic) += .10f;
            player.GetDamage(DamageClass.Summon) += .10f;
            player.statManaMax2 += 50;
            if (Main.zenithWorld)
            {
                player.immune = false;
                player.immuneTime = 0;
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var Hotkey = KeybindSystem.RefresherOrbKeybind.TooltipHotkeyString();
            int index = tooltips.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (index != -1)
            {
                if (Main.zenithWorld)
                {
                    index++;
                    TooltipLine gfb = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<RefresherOrb>("TooltipGFB").Format());
                    tooltips.Insert(index, gfb);
                }
                else
                {
                    index++;
                    TooltipLine normal = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<RefresherOrb>("TooltipNormal").Format(
                    MagicAndSummonDamageBoost.ToPercent(),
                    ManaBoost,
                    Hotkey));
                    tooltips.Insert(index, normal);
                }
            }
        }
        ModKeybind keybindActive = null;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.JungleSpores, 8).
                AddIngredient(ItemID.ChlorophyteBar, 5).
                AddIngredient<FrigidCrystal>(3).
                AddIngredient<ManaCore>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
namespace MogMod.Common.MogModPlayer
{
    public partial class MogPlayer : ModPlayer
    {
        public void RefresherOrbEffect()
        {
            if (KeybindSystem.RefresherOrbKeybind.JustPressed && wearingRefresherOrb && !Player.HasBuff(RefresherOrbCooldown))
            {
                RefresherOrbBuff();
                RefresherOrbDustEffect(Player);
                if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    RefresherOrbSync(Player, false);
                }
            }
        }
        public void RefresherOrbBuff()
        {
            Player.ClearBuff(GlimmerCapeCooldown);
            Player.ClearBuff(SatanicCooldown);
            Player.ClearBuff(ArcaneBootsCooldown);
            Player.ClearBuff(GuardianGreavesCooldown);
            Player.ClearBuff(MekansmCooldown);
            Player.ClearBuff(blademailCooldown);
            Player.ClearBuff(ShivasCooldown);
            Player.ClearBuff(ModContent.BuffType<BlinkDebuff>());
            Player.ClearBuff(ModContent.BuffType<ButterflyCooldown>());
            Player.ClearBuff(ModContent.BuffType<LagunaBladeCooldown>());
            Player.ClearBuff(ModContent.BuffType<ParryCooldown>());
            vindicatorsAxeCooldown = 0;
            mekansmAndGreavesLifeCooldown = 0;
            //Don't add dragon install to this. It shouldn't be able to be refreshed by refresher as it is more of a different mechanic than a buff. Will if you see this stop playing Chen <-- Chen (pronounced "shen") has crazy micro and once i get good at him hes gonna be crazy. that one game was a loss no matter who i played. also it was mendez fault for picking IO

            Player.AddBuff(RefresherOrbCooldown, RefresherOrb.RefresherCooldown);
        }
        public static void RefresherOrbDustEffect(Player player)
        {
            SoundEngine.PlaySound(RefresherActivateSound, player.Center);
            for (int i = 0; i < 150; i++)
            {
                Vector2 shinePos = Main.rand.NextVector2CircularEdge(player.width * 2, player.height);
                Vector2 shineVel = Vector2.UnitY * -Main.rand.NextFloat(-1.5f, 0.5f);
                Dust refreshCircle = Dust.NewDustPerfect(player.Center + shinePos, DustID.JungleTorch, shineVel, 100);
                refreshCircle.fadeIn = 1.45f;
                refreshCircle.velocity *= 0.85f;
                refreshCircle.noGravity = true;
            }
        }
        public void RefresherOrbSync(Player player, bool server)
        {
            ModPacket packet = Mod.GetPacket(256);

            packet.Write((byte)MogModMessageType.RefresherOrbSync);
            packet.Write(player.whoAmI);

            player.SendPacket(packet, server);
        }
        internal void HandleRefresherOrb(BinaryReader reader)
        {
            Player client = reader.ReadPlayer();
            if (Main.netMode == NetmodeID.Server)
            {
                RefresherOrbSync(client, true);
            }
            RefresherOrbDustEffect(client);
        }
    }
}