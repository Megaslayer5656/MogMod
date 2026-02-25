using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class AnglerFish : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Fishing";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 36;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 3);
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.GoodieBags;
        }

        public override bool CanRightClick() => true;
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            int gearMin = 1;
            int gearMax = 1;
            int potionMin = 2;
            int potionMax = 5;
            int otherMin = 1;
            int otherMax = 3;

            // fishing accessories
            itemLoot.Add(ItemDropRule.Common(ItemID.FishermansGuide, 15, gearMin, gearMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.WeatherRadio, 15, gearMin, gearMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.Sextant, 15, gearMin, gearMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.TackleBox, 20, gearMin, gearMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.HighTestFishingLine, 20, gearMin, gearMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.AnglerEarring, 20, gearMin, gearMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.AnglerHat, 25, gearMin, gearMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.AnglerVest, 25, gearMin, gearMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.AnglerPants, 25, gearMin, gearMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.GoldenBugNet, 50, gearMin, gearMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.HotlineFishingHook, 75, gearMin, gearMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.GoldenFishingRod, 100, gearMin, gearMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.ZephyrFish, 200, gearMin, gearMax));

            // potions (and bait)
            itemLoot.Add(ItemDropRule.Common(ItemID.CratePotion, 10, potionMin, potionMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.SonarPotion, 10, potionMin, potionMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.FishingPotion, 10, potionMin, potionMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.LuckPotionLesser, 20, potionMin, potionMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.LuckPotion, 25, potionMin, potionMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.LuckPotionGreater, 30, potionMin, potionMax));

            itemLoot.Add(ItemDropRule.Common(ItemID.ApprenticeBait, 5, potionMin, potionMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.JourneymanBait, 10, potionMin, potionMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.MasterBait, 15, potionMin, potionMax));

            // slop
            itemLoot.Add(ItemDropRule.Common(ItemID.HerbBag, 15, otherMin, otherMax));
            itemLoot.Add(ItemDropRule.Common(ItemID.CanOfWorms, 15, otherMin, otherMax));
        }
    }
}