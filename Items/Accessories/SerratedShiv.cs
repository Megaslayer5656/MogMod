using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class SerratedShiv : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
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
                AddIngredient<BladesOfAttack>(1).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Emblem"}", 1).
                AddRecipeGroup("AdamantiteBar", 18).
                AddIngredient<FuciumBar>(12).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
