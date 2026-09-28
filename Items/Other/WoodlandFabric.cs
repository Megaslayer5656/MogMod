using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class WoodlandFabric : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 3);
        }
        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.Silk, 3).
                AddIngredient(ItemID.BeeWax).
                AddIngredient(ItemID.JungleSpores).
                AddTile(TileID.LivingLoom).
                Register();
        }
    }
}