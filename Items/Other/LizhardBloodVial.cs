using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace MogMod.Items.Other
{
    public class LizhardBloodVial : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 31;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Red;
            Item.value = 1000;
        }
    }
}
