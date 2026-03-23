using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable
{
    public class VoniumBar : ModItem, ILocalizedModType
    {
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
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = Item.useTime = 10;
            Item.autoReuse = true;
            Item.createTile = ModContent.TileType<Tiles.Bars.VoniumBars>();
            Item.placeStyle = 0;
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