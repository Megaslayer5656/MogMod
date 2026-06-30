using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    // greatly reduces fall damage and stops it from killing you (doesnt negate it)
    public class RuntyHorseshoe : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
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
            mogPlayer.wearingRuntyHorseshoe = true;
            player.jumpSpeedBoost += 0.10f;
            player.extraFall += 10;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RuntyBar>(16).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}