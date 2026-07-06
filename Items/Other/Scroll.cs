using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class Scroll : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 5;
        public override void SetDefaults()
        {
            Item.width = Item.height = 36;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 3);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Silk, 3).
                AddIngredient<ManaEssence>(1).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}