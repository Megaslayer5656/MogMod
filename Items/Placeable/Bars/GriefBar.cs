using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable.Bars
{
    public class GriefBar : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 15;
            ItemID.Sets.SortingPriorityMaterials[Type] = 95; // stardust fragment
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Cyan;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = Item.useTime = 10;
            Item.autoReuse = true;
            Item.createTile = ModContent.TileType<Tiles.Bars.GriefBars>();
            Item.placeStyle = 0;
            Item.value = Item.sellPrice(gold: 3, silver: 24);
        }
        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient<HellfireBar>(3).
                AddIngredient<SpookyEssence>(5).
                AddIngredient(ItemID.BeetleHusk, 3).
                AddTile(TileID.AdamantiteForge).
                Register();
        }
    }
}