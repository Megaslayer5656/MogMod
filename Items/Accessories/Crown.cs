using MogMod.Items.Global;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class Crown : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const float DamageBoost = 0.05f;
        public const float AttackSpeedBoost = 0.05f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent(), AttackSpeedBoost.ToPercent());
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 24;
            Item.height = 20;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += DamageBoost;
            player.GetAttackSpeed(DamageClass.Generic) += AttackSpeedBoost;
        }
        /* Moved to Merchant shop.
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.GoldBar, 15).
                AddIngredient(ItemID.PlatinumBar, 10).
                AddIngredient<FrigidShard>(7).
                AddIngredient(ItemID.LargeRuby, 1).
                AddTile(TileID.Anvils).
                Register();
        }
        */
    }
}
