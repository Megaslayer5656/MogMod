using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories;
using MogMod.Items.Ammo;
using MogMod.Items.Weapons.Melee;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
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
            }
            else
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
