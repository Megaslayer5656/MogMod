using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Spirited
{
    [AutoloadEquip(EquipType.Legs)]
    public class SpiritedLeggings : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float MeleeSpeedBoost = 0.08f;
        public const float MovementSpeedBoost = 0.10f;
        public const float JumpSpeedBoost = 0.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeSpeedBoost.ToPercent(), MovementSpeedBoost.ToPercent());
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 5;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed<MeleeDamageClass>() += MeleeSpeedBoost;
            player.moveSpeed += MovementSpeedBoost;
            player.jumpSpeedBoost += JumpSpeedBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => SpiritedHelmet.ModifySetTooltips(this, tooltips);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SpiritShard>(8).
                AddIngredient<ManaEssence>(3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}