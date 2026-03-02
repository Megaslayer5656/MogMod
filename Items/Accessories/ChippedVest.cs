using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class ChippedVest : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.thorns += .8f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Shackle, 1).
                AddIngredient(ItemID.Shuriken, 100).
                AddRecipeGroup(RecipeGroupID.IronBar, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
