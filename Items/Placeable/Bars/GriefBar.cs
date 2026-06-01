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
            Item.createTile = ModContent.TileType<Tiles.Bars.GriefBars>();
            Item.placeStyle = 0;
            Item.value = Item.sellPrice(silver: 88);
        }
        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient(ItemID.HellstoneBar, 4).
                AddIngredient<HellfireEssence>(1).
                AddIngredient(ItemID.SoulofNight, 1).
                AddTile(TileID.AdamantiteForge).
                Register();
        }
    }
}