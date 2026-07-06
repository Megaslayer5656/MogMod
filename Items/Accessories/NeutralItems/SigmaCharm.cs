using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class SigmaCharm : NeutralItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 22;
            Item.height = 34;
            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingSigmaCharm = true;
            mogPlayer.sigmaCharmVisual = !hideVisual;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MoonCharm).
                AddIngredient<DabDadBar>(10).
                AddIngredient<UltimateOrb>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}