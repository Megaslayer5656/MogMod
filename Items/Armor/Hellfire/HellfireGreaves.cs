using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Hellfire
{
    [AutoloadEquip(EquipType.Legs)]
    public class HellfireGreaves : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const int CritBoost = 10;
        public const float MovementBoost = 0.12f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBoost, MovementBoost.ToPercent());
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 22;

            Item.defense = 14;

            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => HellfireMask.ModifySetTooltips(this, tooltips);
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<GenericDamageClass>() += CritBoost;
            player.moveSpeed += MovementBoost;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MoltenGreaves).
                AddIngredient<GriefBar>(12).
                AddIngredient<ScorchedCore>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}