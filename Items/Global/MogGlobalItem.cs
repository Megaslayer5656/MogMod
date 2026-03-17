using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories;
using MogMod.Items.Ammo;
using MogMod.Items.Weapons.Melee;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Global
{
    public class MogGlobalItem : GlobalItem
    {
        public int bloodDamage;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[ItemID.WizardHat] = ModContent.ItemType<GlintstoneArc>();
            ItemID.Sets.ShimmerTransformToItem[ItemID.SparkleGuitar] = ModContent.ItemType<Polylute>();
            ItemID.Sets.ShimmerTransformToItem[ItemID.Frostbrand] = ModContent.ItemType<Flamebrand>();
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<Flamebrand>()] = ItemID.Frostbrand; //So this one doesn't typically work because it can only be obtained by shimmering a frostbrand, and items can only be shimmered once ever according to terraria, so you can't shimmer this into a frostbrand unless you cheat it in and then shimmer it, but I'm leaving this in bc it's funny.
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<ATGMissile>()] = ModContent.ItemType<PlasmaShrimp>();
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<PlasmaShrimp>()] = ModContent.ItemType<ATGMissile>();
        }
        public override void SetDefaults(Item entity)
        {
            if (entity.type == ModContent.ItemType<Reduvia>())
            {
                bloodDamage = 33;

            } else if (entity.type == ModContent.ItemType<Sange>())
            {
                bloodDamage = 110;

            } else if (entity.type == ModContent.ItemType<RiversOfBlood>())
            {
                bloodDamage = 135;

            } else if (entity.type == ModContent.ItemType<Bloodletter>())
            {
                bloodDamage = 15;

            } else if (entity.type == ItemID.PsychoKnife)
            {
               bloodDamage = 95;

            } else if (entity.type == ItemID.BloodButcherer)
            {
                bloodDamage = 16;
            } else
            {
                bloodDamage = 0;
            }
        }
        public override bool InstancePerEntity => true;
        // makes melee weapons size bigger when wearing certain accessories
        public static List<int> MeleeSizeAlwaysAffects =
        [
            ItemID.TerraBlade,
            ItemID.NightsEdge,
            ItemID.TrueNightsEdge,
            ItemID.Excalibur,
            ItemID.TrueExcalibur,
            ItemID.PiercingStarlight,
            ItemID.TheHorsemansBlade,
            ItemID.LucyTheAxe,
            ItemID.TheAxe
        ];
        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
            MogPlayer modPlayer = player.MogMod();

            if (!item.IsAir && !item.noMelee || MeleeSizeAlwaysAffects.Contains(item.type))
            {
                if (modPlayer.wearingSange)
                {
                    scale *= SangeAndYasha.SangeWeaponSize(modPlayer);
                }
                if (modPlayer.wearingGiantsMaul)
                {
                    scale *= GiantsMaul.GiantsMaulWeaponSize(modPlayer);
                }
            }
        }
    }
}
