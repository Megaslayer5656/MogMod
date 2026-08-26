using MogMod.Items.Global;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class HelmOfIronWill : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int DefenseBoost = 1;
        public const int LifeRegenBoost = 2;
        public const int MaxLifeBoost = 20;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxLifeBoost, LifeRegenBoost.ToRegenPerSecond());
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
            Item.defense = DefenseBoost;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.lifeRegen += LifeRegenBoost;
            player.statLifeMax2 += MaxLifeBoost;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("IronBar", 20).
                AddRecipeGroup("AnyGoldBar", 15).
                AddRecipeGroup("AnySilverBar", 12).
                AddIngredient(ItemID.Diamond, 6).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}