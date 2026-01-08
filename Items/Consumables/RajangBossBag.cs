using MogMod.Items.Accessories;
using MogMod.Items.Other;
using MogMod.Items.Weapons;
using MogMod.NPCs.Bosses;
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
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Rajang>()));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ItemID.Banana, 1, 1, 3));
            // what is <>(), chance , stack min, stack max
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<GoldRajangPelt>(), 1, 2, 6));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<RajangApoplexy>(), 2, 1, 4));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<RajangHardHorn>(), 3, 1, 3));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<RajangHardclaw>(), 2, 2, 6));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<GhoulishGoldGorer>(), 3, 1, 2));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<RajangHeart>(), 20, 1, 1));
        }
    }
}
