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
            Item.height = 50;
            Item.width = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Pink;
            Item.value = 8732;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("MythrilBar", 7).
                AddIngredient(ItemID.SoulofLight, 5).
                AddIngredient(ItemID.ManaCrystal, 3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
