using MogMod.Items.Accessories;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Spirited
{

    [AutoloadEquip(EquipType.Body)]
    public class SpiritedBreastplate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float ManaReduction = 0.9f;
        public const int ManaRegenBonus = 2;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaReduction.ToReversedPercent(), ManaRegenBonus.ToRegenPerSecond());
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 18;
            Item.defense = 6;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.manaCost *= ManaReduction;
            player.manaRegenBonus += ManaRegenBonus;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => SpiritedHelmet.ModifySetTooltips(this, tooltips);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SpiritShard>(10).
                AddIngredient<ManaEssence>(3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}