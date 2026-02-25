using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class VoniumBar : ModItem, ILocalizedModType
    {
        // TODO: make this placeable like other bars
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Purple;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VoniumEssence>(3).
                AddIngredient<UltimateOrb>(1).
                AddIngredient(ItemID.LunarBar, 1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}