using MogMod.Common.MogModPlayer;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class OceanHeart : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.LightRed;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();

            // fish slop 1
            mogPlayer.wearingFishSlop1 = true;
            player.accFishingLine = true;
            player.accTackleBox = true;
            player.accFishFinder = true;
            player.accLavaFishing = true;
            player.fishingSkill += 15;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LavaproofTackleBag, 1).
                AddIngredient(ItemID.WoodFishingPole, 1).
                AddIngredient(ItemID.ReinforcedFishingPole, 1).
                AddIngredient(ItemID.BottledWater, 50).
                AddIngredient<FrigidShard>(5).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}