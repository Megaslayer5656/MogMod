using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class SoulFragment : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(10, 3));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 3);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                DisableDecraft().
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Lunar Fragment"}", 3).
                AddIngredient(ItemID.Ectoplasm, 1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}