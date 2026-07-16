using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Items.Armor.Seraphic
{
    [AutoloadEquip(EquipType.Legs)]
    public class SeraphicGreaves : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float MeleeSpeedBoost = 0.25f;
        public const float WhipRangeBoost = 0.2f;
        public const float MovementSpeedBoost = 0.25f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeSpeedBoost.ToPercent(), WhipRangeBoost.ToPercent());
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 16;

            Item.defense = 22;

            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed<MeleeDamageClass>() += MeleeSpeedBoost;
            player.moveSpeed += WhipRangeBoost;
            player.whipRangeMultiplier += MovementSpeedBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.LocalPlayer.armor[0].type == ItemType<SeraphicHelm>())
                SeraphicHelm.ModifySetTooltips(this, tooltips);
            else if (Main.LocalPlayer.armor[0].type == ItemType<SeraphicCrown>())
                SeraphicCrown.ModifySetTooltips(this, tooltips);
        }
        // recipe will be changed eventually
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedGreaves).
                AddIngredient(ItemID.LunarBar, 12).
                AddIngredient<SoulFragment>(3).
                AddIngredient<SoulOfMogMod>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.AncientHallowedGreaves).
                AddIngredient(ItemID.LunarBar, 12).
                AddIngredient<SoulFragment>(3).
                AddIngredient<SoulOfMogMod>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}