using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class BrokenHeroGun : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 22;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 7, silver: 50);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BrokenHeroShard>(5).
                AddIngredient<CraftingRecipe>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}