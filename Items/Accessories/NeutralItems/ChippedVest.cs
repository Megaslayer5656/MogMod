using MogMod.Items.Global;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class ChippedVest : NeutralItem
    {
        public const int DefenseBoost = 2;
        public const float ThornsBoost = 0.8f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ThornsBoost.ToPercent());
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
            Item.defense = DefenseBoost;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.thorns += ThornsBoost;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Shackle).
                AddIngredient(ItemID.Shuriken, 100).
                AddRecipeGroup(RecipeGroupID.IronBar, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}