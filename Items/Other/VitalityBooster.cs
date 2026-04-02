using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class VitalityBooster : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetDefaults()
        {
            Item.height = 50;
            Item.width = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Pink;
            Item.value = 4000;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Bar"}", 5).
                AddIngredient(ItemID.LifeCrystal, 1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
