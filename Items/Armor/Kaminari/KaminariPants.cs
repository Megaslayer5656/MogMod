using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Kaminari
{
    [AutoloadEquip(EquipType.Legs)]
    public class KaminariPants : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float ManaReduction = 1f - 0.2f;
        public const float JumpAndMovementBoost = 0.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaReduction.ToReversedPercent(), JumpAndMovementBoost.ToPercent());
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 20;
            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.manaCost *= ManaReduction;
            player.moveSpeed += JumpAndMovementBoost;
            player.jumpSpeedBoost += JumpAndMovementBoost * 5;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            KaminariHat.ModifySetTooltips(this, tooltips);
            tooltips.IntegrateHotkey(KeybindSystem.ArmorSetBonusKeybind);
            tooltips.IntegrateHotkey(KeybindSystem.ZipSlowdownKeybind);
        }
        ModKeybind keybindActive = null;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RoninPants).
                AddIngredient(ItemID.MartianConduitPlating, 200).
                AddIngredient(ItemID.LunarBar, 12).
                AddIngredient(ItemID.FragmentVortex, 8).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}