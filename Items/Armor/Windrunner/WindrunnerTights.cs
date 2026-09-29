using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Windrunner
{
    [AutoloadEquip(EquipType.Legs)]
    public class WindrunnerTights : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float MovementSpeedBoost = 0.15f;
        public const int RangedCritBoost = 7;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MovementSpeedBoost.ToPercent(), RangedCritBoost);
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 5;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += MovementSpeedBoost;
            player.GetCritChance<RangedDamageClass>() += RangedCritBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            WindrunnerTricorn.ModifySetTooltips(this, tooltips);
            tooltips.IntegrateHotkey(KeybindSystem.ArmorSetBonusKeybind);
        }
        ModKeybind keybindActive = null;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WoodlandFabric>(15).
                AddIngredient<FuciumBar>(10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}