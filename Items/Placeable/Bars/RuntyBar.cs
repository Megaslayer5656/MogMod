using MogMod.Items.Other;
using MogMod.Items.Placeable.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable.Bars
{
    public class RuntyBar : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Type] = 54;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = Item.useTime = 10;
            Item.autoReuse = true;
            Item.createTile = ModContent.TileType<Tiles.Bars.RuntyBars>();
            Item.placeStyle = 0;

            Item.value = Item.sellPrice(copper: 70);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CreepBlood>(1).
                AddIngredient(ItemID.ClayBlock, 5).
                AddTile(TileID.Furnaces).
                Register();
        }
    }
}