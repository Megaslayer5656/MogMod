using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class ManaEssence : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.Sapphire, 1).
                AddIngredient(ItemID.ManaCrystal, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}