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
        public const int DefenseBoost = 2;
        public const float DamageReductionBoost = 0.05f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageReductionBoost.ToPercent());
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
            player.endurance += DamageReductionBoost;
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