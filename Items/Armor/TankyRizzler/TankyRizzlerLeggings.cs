using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.TankyRizzler
{
    [AutoloadEquip(EquipType.Legs)]
    public class TankyRizzlerLeggings : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float MeleeSpeedBoost = 0.10f;
        public const float MovementSpeedBoost = 0.10f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeSpeedBoost.ToPercent());
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 20;
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed<MeleeDamageClass>() += MeleeSpeedBoost;
            player.moveSpeed += MovementSpeedBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => TankyRizzlerHelmet.ModifySetTooltips(this, tooltips);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BeetleLeggings, 1).
                AddIngredient(ItemID.MartianConduitPlating, 75).
                AddIngredient<UltimateOrb>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}