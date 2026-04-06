using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories;
using MogMod.Items.Ammo;
using MogMod.Items.Other;
using MogMod.Items.Weapons.Magic;
using MogMod.Items.Weapons.Melee;
using MogMod.Items.Weapons.Ranged;
using MogMod.Rarities;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using System.Linq;
using Terraria.GameContent.ItemDropRules;
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
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            // Check if the opened bag is the Eye of Cthulhu Treasure Bag
            if (item.type == ItemID.FishronBossBag)
            {
                itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<BrinyRind>(), 1, 9, 16));
            }
        }
        // damascus crit damage increase
        public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            if (mogPlayer.wearingDamascus1)
                modifiers.CritDamage *= 1.2f;
        }
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            if (mogPlayer.wearingDamascus2 && hit.Crit)
            {
                int heal = 1;
                heal *= Convert.ToInt32(player.lifeSteal * 0.02);
                player.statLife += heal;
                player.HealEffect(heal);
                if (player.statLife > player.statLifeMax2)
                    player.statLife = player.statLifeMax2;
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
            ModContent.ItemType<Gunlance>(),
            ItemID.TheAxe
        ];
        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
            MogPlayer modPlayer = player.MogMod();

            if (!item.IsAir && !item.noMelee || MeleeSizeAlwaysAffects.Contains(item.type))
            {
                if (modPlayer.wearingGiantsMaul)
                    scale *= GiantsMaul.GiantsMaulWeaponSize(modPlayer);
            }
        }

        // taken from calamity mods rarity price system
        // these values should NOT be used for materials, potions, and ammo
        #region Rarity Price Table
        // Base numeric rarity pricing guide.
        private static readonly int Rarity0BuyPrice = Item.buyPrice(0, 0, 50, 0);
        private static readonly int Rarity1BuyPrice = Item.buyPrice(0, 1, 0, 0);
        private static readonly int Rarity2BuyPrice = Item.buyPrice(0, 2, 0, 0);
        private static readonly int Rarity3BuyPrice = Item.buyPrice(0, 5, 0, 0);
        private static readonly int Rarity4BuyPrice = Item.buyPrice(0, 10, 0, 0);
        private static readonly int Rarity5BuyPrice = Item.buyPrice(0, 20, 0, 0);
        private static readonly int Rarity6BuyPrice = Item.buyPrice(0, 35, 0, 0);
        private static readonly int Rarity7BuyPrice = Item.buyPrice(0, 45, 0, 0);
        private static readonly int Rarity8BuyPrice = Item.buyPrice(0, 60, 0, 0);
        private static readonly int Rarity9BuyPrice = Item.buyPrice(0, 80, 0, 0);
        private static readonly int Rarity10BuyPrice = Item.buyPrice(1, 0, 0, 0); // Highest raw rarity used by vanilla items (ML drops)
        private static readonly int Rarity11BuyPrice = Item.buyPrice(1, 20, 0, 0); // End of vanilla rarities
        private static readonly int Rarity12BuyPrice = Item.buyPrice(1, 50, 0, 0); // Von rarity

        private static readonly int[] RarityBuyPriceArray = new int[] {
            Rarity0BuyPrice,
            Rarity1BuyPrice,
            Rarity2BuyPrice,
            Rarity3BuyPrice,
            Rarity4BuyPrice,
            Rarity5BuyPrice,
            Rarity6BuyPrice,
            Rarity7BuyPrice,
            Rarity8BuyPrice,
            Rarity9BuyPrice,
            Rarity10BuyPrice,
            Rarity11BuyPrice,
            Rarity12BuyPrice,
        };

        // Canonical names which are implemented as properties that reference the base numeric rarity prices.
        public static int RarityWhiteBuyPrice => Rarity0BuyPrice;
        public static int RarityBlueBuyPrice => Rarity1BuyPrice;
        public static int RarityGreenBuyPrice => Rarity2BuyPrice;
        public static int RarityOrangeBuyPrice => Rarity3BuyPrice;
        public static int RarityLightRedBuyPrice => Rarity4BuyPrice;
        public static int RarityPinkBuyPrice => Rarity5BuyPrice;
        public static int RarityLightPurpleBuyPrice => Rarity6BuyPrice;
        public static int RarityLimeBuyPrice => Rarity7BuyPrice;
        public static int RarityYellowBuyPrice => Rarity8BuyPrice;
        public static int RarityCyanBuyPrice => Rarity9BuyPrice;
        public static int RarityRedBuyPrice => Rarity10BuyPrice;
        public static int RarityPurpleBuyPrice => Rarity11BuyPrice;
        public static int RarityVonBuyPrice => Rarity12BuyPrice;
        #endregion

        #region Rarity / Price Helper Functions
        public static int GetBuyPrice(int rarity)
        {
            // Vanilla rarities go directly to the array.
            if (rarity >= ItemRarityID.White && rarity <= ItemRarityID.Purple)
                return RarityBuyPriceArray[rarity];

            // modded rarities aren't guaranteed to have the monotonic IDs, so they're handled directly.
            if (rarity == ModContent.RarityType<VonRarity>())
                return RarityVonBuyPrice;

            // Return 0 if it's not a progression based or other mod's rarity
            return 0;
        }

        public static int GetBuyPrice(Item item) => GetBuyPrice(item.rare);
        #endregion

        #region Custom Rarity Colors
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // Apply rarity coloration to the item's name.
            TooltipLine nameLine = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria");
            if (nameLine != null)
                ApplyRarityColor(item, nameLine);
        }
        private void ApplyRarityColor(Item item, TooltipLine nameLine)
        {
            if (item.type == ModContent.ItemType<AghanimBlessing>())
            {
                nameLine.OverrideColor = MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f % 1f, new Color[]
                {
                    new Color(34, 27, 194),
                    new Color(183, 27, 194),
                    new Color(194, 27, 83)
                });
            }
            if (item.type == ModContent.ItemType<DivineRapierWeapon>())
            {
                nameLine.OverrideColor = MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f % 1f, new Color[]
                {
                    new Color(250, 231, 200),
                    new Color(200, 250, 224),
                    new Color(243, 200, 250)
                });
            }
            if (item.type == ModContent.ItemType<Megaslark>())
            {
                nameLine.OverrideColor = MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f % 1f, new Color[]
                {
                    new Color(82, 156, 25),
                    new Color(25, 156, 106),
                    new Color(25, 106, 156)
                });
            }
        }
        #endregion
    }
}
