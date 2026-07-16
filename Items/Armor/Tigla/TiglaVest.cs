using MogMod.Common.MogModPlayer;
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
    [AutoloadEquip(EquipType.Body)]
    public class TiglaVest : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float RangedDamageBoost = 0.16f;
        public const int RangedCritBoost = 16;
        public const float AmmoReduction = 0.8f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RangedCritBoost, AmmoReduction.ToReversedPercent());
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 14;
            Item.defense = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RangedDamageClass>() += RangedDamageBoost;
            player.GetCritChance<RangedDamageClass>() += RangedCritBoost;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.ammoCost *= AmmoReduction;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => TiglaHelmet.ModifySetTooltips(this, tooltips);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ShroomiteBreastplate, 1).
                AddIngredient(ItemID.Cog, 125).
                AddIngredient<DabDadBar>(16).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}