using Microsoft.Xna.Framework;
using MogMod.Buffs.Cooldowns;
using MogMod.Buffs.Debuffs;
using MogMod.Common.MogModPlayer;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            if (player.statLife >= (player.statLifeMax2 * 1))
            {
                mogPlayer.isWearingGlimmerCape = true;
                mogPlayer.wearingManaBoots = true;
                mogPlayer.wearingSatanic = true;
                mogPlayer.wearingRefresherOrb = true;
                mogPlayer.locketActive = true;
                mogPlayer.wandActive = true;
                mogPlayer.stickActive = true;
                mogPlayer.armletActive = true;
                mogPlayer.wearingHelmOfDominator = true;
                mogPlayer.wearingHelmOfOverlord = true;
                mogPlayer.wearingGigaManaBoots = true;
                mogPlayer.wearingMekansm = true;
                mogPlayer.wearingForceStaff = true;
                mogPlayer.wearingPike = true;
                mogPlayer.wearingShivasGuard = true;
                mogPlayer.wearingEyeOfSkadi = true;
                mogPlayer.wearingFlameOfCorruption = true;
                mogPlayer.wearingWingsOfLight = true;
                mogPlayer.wingsOfLightVisual = true;
                mogPlayer.wearingFishSlop1 = true;
                mogPlayer.wearingFishSlop2 = true;
                mogPlayer.wearingGiantsMaul = true;
                mogPlayer.wearingGunpowderGauntlet = true;
                mogPlayer.wearingDuelistGloves = true;
                mogPlayer.wearingWhisperDread = true;
                mogPlayer.wearingSerratedShiv = true;
                mogPlayer.wearingUndyingHelm = true;
                mogPlayer.wearingSearingSignet = true;
                mogPlayer.wearingVladimirs = true;
                mogPlayer.wearingWraithPact = true;
                mogPlayer.wearingJidiPollenBag = true;
                mogPlayer.wearingShadowAmulet = true;
                mogPlayer.shadowAmuletVisual = true;
                mogPlayer.exultationEquipped = true;
                mogPlayer.plasmaVisual = true;
                mogPlayer.polyluteVisual = true;
                mogPlayer.wearingRuntyHorseshoe = true;
                mogPlayer.wraithActive = true;
                mogPlayer.wearingRadiantArmor = true;
                mogPlayer.wearingUndyingArmor = true;
                mogPlayer.wearingTankyRizzler = true;
                mogPlayer.wearingBladeMail = true;
                mogPlayer.wearingFrostArmor = true;
                mogPlayer.wearingFrostMagic = true;
                mogPlayer.wearingFrostSummon = true;
                mogPlayer.wearingDamascus1 = true;
                mogPlayer.wearingDamascus2 = true;
                mogPlayer.wearingBoneArmor = true;
                mogPlayer.wearingWhiteArmor = true;
                mogPlayer.wearingFaeArmor = true;
                mogPlayer.wearingHellfireArmor = true;
                mogPlayer.wearingSpiritArmor = true;
                mogPlayer.wearingSeraphic = true;
                mogPlayer.canSeraphicRevive = true;
                mogPlayer.wearingNihilum = true;
                mogPlayer.wearingNihilumRanged = true;
                mogPlayer.diademMinion = true;
                mogPlayer.dominatorMinion = true;
                mogPlayer.overlordMinion = true;
                mogPlayer.infiniteFlight = true;
                mogPlayer.greavesAura = true;
                mogPlayer.wraithAura = true;
                mogPlayer.vladsAura = true;
                mogPlayer.headdressAura = true;
                mogPlayer.drumsAura = true;
                mogPlayer.shivasAura = true;
                mogPlayer.ahmodPet = true;
                mogPlayer.inShadowRealm = true;
                mogPlayer.atgActive = true;
                mogPlayer.plasmaActive = true;
                mogPlayer.icbmActive = true;
                mogPlayer.polyluteActive = true;
                mogPlayer.holdingThrowingShade = true;
                mogPlayer.ammoCost *= 0f;
                mogPlayer.fCrystal = true;
                mogPlayer.divinitasMinion = true;
            }
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
            player.endurance += 5f;
        }
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip1");
            if (line != null)
                line.OverrideColor = Main.DiscoColor;
        }
    }
}