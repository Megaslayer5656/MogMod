using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Runty
{
    [AutoloadEquip(EquipType.Legs)]
    public class RuntyGreaves : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float MovementSpeedBoost = 0.08f;
        public const float JumpSpeedBoost = 0.4f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MovementSpeedBoost.ToPercent());
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;

            Item.defense = 3;

            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += MovementSpeedBoost;
            player.jumpSpeedBoost += JumpSpeedBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => RuntyHelmet.ModifySetTooltips(this, tooltips);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RuntyBar>(12).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}