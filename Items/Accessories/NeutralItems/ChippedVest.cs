using MogMod.Items.Global;
using Terraria;
using Terraria.ID;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class ChippedVest : NeutralItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
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