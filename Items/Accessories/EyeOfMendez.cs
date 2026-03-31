using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;

namespace MogMod.Items.Accessories
{
    public class EyeOfMendez : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Master;
            Item.value = Item.buyPrice(1000, 0, 0, 67);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingMendez = true;
            mogPlayer.wearingBladeMail = true;
            mogPlayer.wearingDuelistGloves = true;
            mogPlayer.wearingEyeOfSkadi = true;
            mogPlayer.wearingFishSlop1 = true;
            mogPlayer.wearingFishSlop2 = true;
            mogPlayer.wearingFlameOfCorruption = true;
            mogPlayer.wearingForceStaff = true;
            mogPlayer.wearingGiantsMaul = true;
            mogPlayer.wearingGigaManaBoots = true;
            mogPlayer.wearingGunpowderGauntlet = true;
            mogPlayer.wearingHelmOfDominator = true;
            mogPlayer.wearingHelmOfOverlord = true;
            mogPlayer.wearingJidiPollenBag = true;
            mogPlayer.wearingManaBoots = true;
            mogPlayer.wearingMekansm = true;
            mogPlayer.wearingPike = true;
            mogPlayer.wearingRadiantArmor = true;
            mogPlayer.wearingRefresherOrb = true;
            mogPlayer.wearingSatanic = true;
            mogPlayer.wearingSearingSignet = true;
            mogPlayer.wearingSerratedShiv = true;
            mogPlayer.wearingShadowAmulet = true;
            mogPlayer.wearingShivasGuard = true;
            mogPlayer.wearingUndyingArmor = true;
            mogPlayer.wearingUndyingHelm = true;
            mogPlayer.wearingVladimirs = true;
            mogPlayer.wearingWhisperDread = true;
            mogPlayer.wearingWingsOfLight = true;
            mogPlayer.wearingWraithPact = true;
            mogPlayer.isWearingGlimmerCape = true;
            mogPlayer.plasmaActive = true;
            mogPlayer.atgActive = true;
            mogPlayer.icbmActive = true;
            mogPlayer.polyluteActive = true;
            mogPlayer.wandActive = true;
            mogPlayer.stickActive = true;
            mogPlayer.locketActive = true;
            mogPlayer.armletActive = true;
            mogPlayer.drumsAura = true;
            mogPlayer.greavesAura = true;
            mogPlayer.headdressAura = true;
            mogPlayer.shivasAura = true;
            mogPlayer.vladsAura = true;
            mogPlayer.wraithAura = true;
            mogPlayer.exultationEquipped = true;

            player.pickSpeed *= 0f;
            player.tileSpeed *= 100f;
            player.blockRange += 300;
            player.statLifeMax2 += 1500;
            player.statManaMax2 += 2000;
            player.GetDamage(DamageClass.Generic) += 25f;
            player.GetAttackSpeed(DamageClass.Generic) += 50f;
            player.maxMinions += 500;
            player.maxTurrets += 500;
            player.statDefense += 500;
            player.aggro += -2500;
            player.endurance *= 5f;
        }
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            List<Color> colorList = new List<Color>()
            {
                Color.Red,
                Color.Orange,
                Color.Yellow,
                Color.Green,
                Color.Blue,
                Color.Purple
            };

            int colorIndex = (int)(Main.GlobalTimeWrappedHourly / 2 % colorList.Count);
            Color currentColor = colorList[colorIndex];
            Color nextColor = colorList[(colorIndex + 1) % colorList.Count];
            Color tooltipColor = Color.Lerp(currentColor, nextColor, Main.GlobalTimeWrappedHourly % 2f > 1f ? 1f : Main.GlobalTimeWrappedHourly % 1f);

            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip1");
            if (line != null)
                line.OverrideColor = Color.Lerp(tooltipColor, Color.White, 0.5f);
        }
    }
}
