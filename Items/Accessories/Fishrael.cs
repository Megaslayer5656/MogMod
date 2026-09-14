using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class Fishrael : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int FishingPowerBoost = 60;
        public const int LureCount = 10;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FishingPowerBoost, LureCount);
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 42;
            Item.height = 54;
            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();

            // fish slop 1
            mogPlayer.wearingFishSlop1 = true;
            player.accFishingLine = true;
            player.accTackleBox = true;
            player.accFishFinder = true;
            player.accLavaFishing = true;

            // fish slop 2
            mogPlayer.wearingFishSlop2 = true;
            player.fishingSkill += FishingPowerBoost;
            player.sonarPotion = true;
            player.cratePotion = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<OceanHeart>().
                AddIngredient(ItemID.GoldenFishingRod).
                AddIngredient(ItemID.FishingPotion, 15).
                AddIngredient(ItemID.CratePotion, 15).
                AddIngredient(ItemID.SonarPotion, 15).
                AddIngredient<CraftingRecipe>(3).
                AddIngredient(ItemID.PlatinumCoin).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}