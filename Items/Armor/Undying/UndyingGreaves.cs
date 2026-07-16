using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Undying
{
    [AutoloadEquip(EquipType.Legs)]
    public class UndyingGreaves : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float MovementBoost = 0.2f;
        public const int CritBoost = 12;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MovementBoost.ToPercent(), CritBoost);
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;

            Item.defense = 18;

            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => UndyingHelm.ModifySetTooltips(this, tooltips);
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<GenericDamageClass>() += CritBoost;
            player.moveSpeed += MovementBoost;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BrinyRind>(7).
                AddIngredient<UltimateOrb>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}