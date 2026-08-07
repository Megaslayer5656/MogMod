using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class SerratedShiv : NeutralItem
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int DamageCap = 400;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = MogGlobalItem.RarityLightPurpleBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingSerratedShiv = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BladesOfAttack>().
                AddRecipeGroup("AnyEmblem").
                AddRecipeGroup("AnyAdamantiteBar", 18).
                AddIngredient<FuciumBar>(12).
                AddIngredient(ItemID.SoulofFright, 7).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}