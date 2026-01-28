using MogMod.Common.MogModPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class Diadem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.diademMinion = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("GoldBar", 15).
                AddIngredient(ItemID.ManaCrystal, 5).
                AddIngredient(ItemID.Sapphire, 7).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Bar"}", 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
