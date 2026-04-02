using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class FrigidShard : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 15;
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 14;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.IceBlock, 9).
                AddIngredient(ItemID.Glass, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}