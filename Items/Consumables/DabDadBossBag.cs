using MogMod.Items.Accessories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Consumables
{
    public class DabDadBossBag : ModItem
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
            Item.rare = ItemRarityID.Purple;
        }
        public override bool CanRightClick()
        {
            return true;
        }
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<DesperationCharm>()));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<ETGC>(), 1, 1, 5));
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<DabdadWings>(), 0, 1, 1));
        }
    }
}
