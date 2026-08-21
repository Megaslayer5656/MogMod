using MogMod.Items.Global;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    [AutoloadEquip(EquipType.HandsOn)]
    public class SolRing : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int MaxManaBoost = 40;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxManaBoost);
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 28;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue; // i'd like to keep the rarities similar to base vanilla rarities (refer to https://terraria.wiki.gg/wiki/Rarity)
            Item.value = MogGlobalItem.RarityBlueBuyPrice; // it also makes rarity prices consistent (pre hardmode item costing 1 plat would be crazy)
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statManaMax2 += MaxManaBoost;
            float dim = .01f;
            Lighting.AddLight(player.Center, 25 * dim, 23 * dim, 11 * dim);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
            AddRecipeGroup("AnyGoldBar", 6).
            AddIngredient(ItemID.FallenStar, 6).
            AddTile(TileID.Anvils).
            Register();
        }
    }
}