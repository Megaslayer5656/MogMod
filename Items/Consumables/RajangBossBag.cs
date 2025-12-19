using MogMod.Items.Accessories;
using MogMod.Items.Other;
using MogMod.Items.Weapons;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Consumables
{
    public class RajangBossBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override bool CanRightClick()
        {
            return true;
        }
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // switch the switch to vons switch
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Switch>()));
            // what is <ETGC>(), 1, 1, 5 ??
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<ETGC>(), 1, 1, 5));
        }
    }
}
