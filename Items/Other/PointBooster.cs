using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class PointBooster : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetDefaults()
        {
            Item.height = Item.width = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 7);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyMythrilBar", 5).
                AddIngredient(ItemID.SoulofLight, 3).
                AddIngredient(ItemID.ManaCrystal).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}