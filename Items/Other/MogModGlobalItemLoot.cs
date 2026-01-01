using MogMod.Items.Accessories;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
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
            switch (item.type)
            {
                case ItemID.WoodenCrate:
                    loot.Add(ItemDropRule.Common(ModContent.ItemType<CraftingRecipe>()));
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(5, 1,
                    ModContent.ItemType<BladesOfAttack>(),
                    ItemID.WandofSparking));
                    break;

                case ItemID.WoodenCrateHard:
                    loot.Add(ItemDropRule.Common(ModContent.ItemType<CraftingRecipe>()));
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(3, 1,
                    ModContent.ItemType<BladesOfAttack>(),
                    ItemID.WandofSparking));
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(10, 1,
                    ModContent.ItemType<VladmirsOffering>(),
                    ModContent.ItemType<ArmletOfMordiggian>()));
                    break;


                case ItemID.IronCrate:
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(5, 1,
                    ModContent.ItemType<BeltOfStrength>()));
                    break;

                case ItemID.IronCrateHard:
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(3, 1,
                    ModContent.ItemType<BeltOfStrength>()));
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(10, 1,
                    ModContent.ItemType<DragonLance>(),
                    ModContent.ItemType<DrumOfEndurance>()));
                    break;
            }
        }
    }
}