using MogMod.Items.Global;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    [AutoloadEquip(EquipType.HandsOn)]
    public class BeltOfStrength : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const float DamageBoost = 0.05f;
        public const float KnockbackBoost = 0.10f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent(), KnockbackBoost.ToPercent());
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 30;
            Item.height = 26;
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += DamageBoost;
            player.GetKnockback(DamageClass.Generic) += KnockbackBoost;
        }
    }
}