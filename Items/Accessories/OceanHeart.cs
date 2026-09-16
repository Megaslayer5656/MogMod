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
    public class OceanHeart : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int FishingPowerBoost = 15;
        public const int LureCount = 6;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FishingPowerBoost, LureCount);
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
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
            player.fishingSkill += FishingPowerBoost;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LavaproofTackleBag).
                AddIngredient(ItemID.WoodFishingPole).
                AddIngredient(ItemID.ReinforcedFishingPole).
                AddIngredient(ItemID.BottledWater, 50).
                AddIngredient<FrigidShard>(5).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}