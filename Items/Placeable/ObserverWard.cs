using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable
{
    public class ObserverWard : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Torches[Type] = true;
            ItemID.Sets.SingleUseInGamepad[Type] = true;
        }

        public override void SetDefaults()
        {
            // DefaultToTorch sets various properties common to torch placing items. Hover over DefaultToTorch in Visual Studio to see the specific properties set.
            // Of particular note to torches are Item.holdStyle, Item.flame, and Item.noWet. 
            Item.DefaultToTorch(ModContent.TileType<Tiles.ObserverWardTile>(), 0, true);
            Item.value = 50;
            Item.scale = 1f;
        }

        public override void HoldItem(Player player)
        {
            return;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
             AddIngredient(ItemID.Torch, 20).
             AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Material"}", 2).
             AddIngredient(ItemID.Lens, 1).
             AddTile(TileID.DemonAltar).
             Register();
        }
    }
}
