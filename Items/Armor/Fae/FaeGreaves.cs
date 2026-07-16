using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Fae
{
    [AutoloadEquip(EquipType.Legs)]
    public class FaeGreaves : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float AttackSpeedBoost = 0.12f;
        public const float MovementSpeedBoost = 0.24f;
        public const int MaxMinions = 2;
        public const int MaxSentries = 2;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MovementSpeedBoost.ToPercent(), AttackSpeedBoost.ToPercent(), MaxMinions, MaxSentries);
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;

            Item.defense = 16;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Generic) += AttackSpeedBoost;
            player.moveSpeed += MovementSpeedBoost;
            player.maxMinions += MaxMinions;
            player.maxTurrets += MaxSentries;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => FaeMask.ModifySetTooltips(this, tooltips);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FaeBar>(12).
                AddIngredient(ItemID.CrystalNinjaLeggings, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}