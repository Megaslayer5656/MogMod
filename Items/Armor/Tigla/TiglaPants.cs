using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Tigla
{
    [AutoloadEquip(EquipType.Legs)]
    public class TiglaPants : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const int RangedCritBoost = 9;
        public const float MovementSpeedBoost = 0.15f;
        public const float JumpSpeedBoost = 0.75f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RangedCritBoost, MovementSpeedBoost.ToPercent());
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 22;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Ranged) += RangedCritBoost;
            player.moveSpeed += MovementSpeedBoost;
            player.jumpSpeedBoost += JumpSpeedBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => TiglaHelmet.ModifySetTooltips(this, tooltips);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ShroomiteLeggings, 1).
                AddIngredient(ItemID.Cog, 75).
                AddIngredient<DabDadBar>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}