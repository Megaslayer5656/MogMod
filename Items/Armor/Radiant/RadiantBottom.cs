using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Radiant
{
    [AutoloadEquip(EquipType.Legs)]
    public class RadiantBottom : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float MagicDamageBoost = 0.1f;
        public const float MovementSpeedBoost = 0.12f;
        public const float JumpSpeedBoost = 0.6f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MagicDamageBoost.ToPercent(), MovementSpeedBoost.ToPercent());
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 26;
            Item.defense = 11;
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MagicDamageClass>() += MagicDamageBoost;
            player.moveSpeed += MovementSpeedBoost;
            player.jumpSpeedBoost += JumpSpeedBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => RadiantFlower.ModifySetTooltips(this, tooltips);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpectrePants, 1).
                AddIngredient<FaeBar>(14).
                AddIngredient<ManaCore>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}