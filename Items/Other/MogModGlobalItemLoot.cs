using MogMod.Items.Accessories;
using MogMod.Items.Placeable;
using MogMod.Items.Weapons.Magic;
using MogMod.Items.Weapons.Melee;
using MogMod.Utilities;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class MogModGlobalItemLoot : GlobalItem
    {
        public override bool InstancePerEntity => false;
        public override void ModifyItemLoot(Item item, ItemLoot loot)
        {
            LeadingConditionRule postEvil = loot.DefineConditionalDropSet(DropHelper.PostEvil());
            LeadingConditionRule postEoL = loot.DefineConditionalDropSet(DropHelper.PostEoL());
            switch (item.type)
            {
                case ItemID.WoodenCrate:
                    loot.Add(ItemDropRule.Common(ModContent.ItemType<CraftingRecipe>(), 3, 1, 3));
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(20, 1,
                        ModContent.ItemType<BladesOfAttack>(),
                        ItemID.WandofSparking));
                    break;

                case ItemID.WoodenCrateHard:
                    loot.Add(ItemDropRule.Common(ModContent.ItemType<CraftingRecipe>(), 3, 1, 3));
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(20, 1,
                        ModContent.ItemType<BladesOfAttack>(),
                        ItemID.WandofSparking));
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(50, 1,
                        ModContent.ItemType<VladmirsOffering>(),
                        ModContent.ItemType<ArmletOfMordiggian>()));
                    break;


                case ItemID.IronCrate:
                    postEvil.Add(ItemDropRule.Common(ModContent.ItemType<FuciumOre>(), 6, 8, 14));
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(20, 1,
                        ModContent.ItemType<BeltOfStrength>(),
                        ModContent.ItemType<AstrologersStaff>()));
                    break;

                case ItemID.IronCrateHard:
                    loot.Add(ItemDropRule.Common(ModContent.ItemType<FuciumOre>(), 6, 12, 18));
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(20, 1,
                        ModContent.ItemType<BeltOfStrength>(),
                        ModContent.ItemType<AstrologersStaff>()));
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(50, 1,
                        ModContent.ItemType<DragonLance>(),
                        ModContent.ItemType<DrumOfEndurance>()));
                    break;


                case ItemID.GoldenCrate:
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(50, 1,
                        ModContent.ItemType<BizarreMusicBox>()));
                    postEvil.Add(new OneFromOptionsNotScaledWithLuckDropRule(50, 1,
                        ModContent.ItemType<BootsOfTravel>()));
                    break;

                case ItemID.GoldenCrateHard:
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(20, 1, ItemID.MedusaHead));
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(50, 1,
                        ModContent.ItemType<UltraBootsOfTravel>(),
                        ModContent.ItemType<BizarreMusicBox>()));
                    break;


                case ItemID.FrozenCrate:
                    loot.Add(ItemDropRule.Common(ModContent.ItemType<FrigidShard>(), 5, 3, 5));
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(20, 1,
                        ModContent.ItemType<FrozenSpear>(),
                        ItemID.WandofFrosting));
                    break;

                case ItemID.FrozenCrateHard:
                    loot.Add(ItemDropRule.Common(ModContent.ItemType<FrigidShard>(), 5, 3, 5));
                    loot.Add(ItemDropRule.Common(ModContent.ItemType<FrigidCrystal>(), 7, 1, 3));
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(20, 1,
                        ModContent.ItemType<FrozenSpear>(),
                        ItemID.WandofFrosting));
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(50, 1,
                        ModContent.ItemType<GlimmerCape>()));
                    break;


                case ItemID.HallowedFishingCrate:
                    postEoL.Add(ItemDropRule.Common(ModContent.ItemType<FaeOre>(), 6, 8, 14));
                    break;

                case ItemID.HallowedFishingCrateHard:
                    postEoL.Add(ItemDropRule.Common(ModContent.ItemType<FaeOre>(), 6, 12, 18));
                    loot.Add(ItemDropRule.Common(ModContent.ItemType<PointBooster>(), 7, 1, 1));
                    break;
            }
        }
    }
}