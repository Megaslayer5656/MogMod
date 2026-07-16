using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Nihilum
{
    [AutoloadEquip(EquipType.Legs)]
    public class NihilumLeggings : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float JumpAndMovementBoost = 0.3f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(JumpAndMovementBoost.ToPercent());
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 18;

            Item.defense = 20;

            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            NihilumHeadgear.ModifySetTooltips(this, tooltips);
            tooltips.IntegrateHotkey(KeybindSystem.NulledKeybind);
        }
        ModKeybind keybindActive = null;
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += JumpAndMovementBoost;
            player.jumpSpeedBoost += JumpAndMovementBoost * 5;
        }
        // recipe will be changed eventually
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ChlorophyteGreaves).
                AddIngredient(ItemID.LunarBar, 12).
                AddIngredient<SoulFragment>(3).
                AddIngredient<SoulOfMogMod>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}