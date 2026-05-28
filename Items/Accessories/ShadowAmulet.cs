using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class ShadowAmulet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public int i;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingShadowAmulet = true;
            mogPlayer.shadowAmuletVisual = !hideVisual;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Amethyst, 5).
                AddIngredient(ItemID.Sapphire, 1).
                AddIngredient(ItemID.InvisibilityPotion, 1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
