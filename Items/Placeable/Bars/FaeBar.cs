using MogMod.Items.Other;
using MogMod.Items.Placeable.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable.Bars
{
    public class FaeBar : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Type] = 90; // Chlorophyte Ore
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Pink;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = Item.useTime = 10;
            Item.autoReuse = true;
            Item.createTile = ModContent.TileType<Tiles.Bars.FaeBars>();
            Item.placeStyle = 0;

            Item.value = Item.sellPrice(silver: 132);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FaeOre>(4).
                AddIngredient(ItemID.PixieDust, 1).
                AddTile(TileID.AdamantiteForge).
                Register();
        }
    }
}