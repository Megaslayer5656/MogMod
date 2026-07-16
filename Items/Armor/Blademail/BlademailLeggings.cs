using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Blademail
{
    [AutoloadEquip(EquipType.Legs)]
    public class BlademailLeggings : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float MeleeSpeedBoost = 0.1f;
        public const float MovementSpeedBoost = 0.1f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeSpeedBoost.ToPercent());
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 9;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed<MeleeDamageClass>() += MeleeSpeedBoost;
            player.moveSpeed += MovementSpeedBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            BlademailMask.ModifySetTooltips(this, tooltips);
            tooltips.IntegrateHotkey(KeybindSystem.BladeMailKeybind);
        }
        ModKeybind keybindActive = null;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Spike, 30).
                AddIngredient(ItemID.Bone, 40).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}