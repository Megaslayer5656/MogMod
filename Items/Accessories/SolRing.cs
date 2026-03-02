using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class SolRing : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 32;
            Item.height = 41;
            Item.rare = ItemRarityID.Blue; // i'd like to keep the rarities similar to base vanilla rarities (refer to https://terraria.wiki.gg/wiki/Rarity)
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statManaMax2 += 40;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
            AddRecipeGroup("GoldBar", 6).
            AddIngredient(ItemID.FallenStar, 6).
            AddTile(TileID.Anvils).
            Register();
        }
    }
}
