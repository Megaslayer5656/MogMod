using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class CraftingRecipe : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(silver: 3);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<IronBranch>(1).
                AddIngredient(ItemID.CopperCoin, 5).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}