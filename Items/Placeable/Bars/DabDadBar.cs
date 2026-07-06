using MogMod.Items.Placeable.Ores;
using MogMod.Tiles.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable.Bars
{
    public class DabDadBar : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Type] = 90; // Chlorophyte Ore
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 99;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.createTile = ModContent.TileType<DabDadBars>();
            Item.placeStyle = 0;

            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 1, silver: 20);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DabDadOreP>(5).
                AddTile(TileID.AdamantiteForge).
                Register();
        }
    }
}
