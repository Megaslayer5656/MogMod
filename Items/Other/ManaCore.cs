using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class ManaCore : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Pink;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient<ManaEssence>(3).
                AddIngredient<UltimateOrb>(1).
                AddIngredient(ItemID.UnicornHorn, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}