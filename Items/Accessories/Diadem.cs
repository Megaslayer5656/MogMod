using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class Diadem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int MaxMinions = 1;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxMinions);
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = Item.height = 24;
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.diademMinion = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyGoldBar", 15).
                AddIngredient(ItemID.Sapphire, 7).
                AddIngredient<FrigidShard>(5).
                AddRecipeGroup("AnyEvilBar", 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
