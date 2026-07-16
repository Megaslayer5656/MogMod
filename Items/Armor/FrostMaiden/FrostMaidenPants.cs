using MogMod.Items.Global;
using MogMod.Items.Other;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Items.Armor.FrostMaiden
{
    [AutoloadEquip(EquipType.Legs)]
    public class FrostMaidenPants : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const int MaxMinions = 2;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxMinions);
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 18;
            Item.defense = 7;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.maxMinions += MaxMinions;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.LocalPlayer.armor[0].type == ItemType<FrostMaidenMagic>())
                FrostMaidenMagic.ModifySetTooltips(this, tooltips);
            else if (Main.LocalPlayer.armor[0].type == ItemType<FrostMaidenSummon>())
                FrostMaidenSummon.ModifySetTooltips(this, tooltips);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Bone, 25).
                AddIngredient<FrigidShard>(5).
                AddIngredient(ItemID.FlinxFur, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}