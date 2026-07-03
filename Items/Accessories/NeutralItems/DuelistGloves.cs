using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class DuelistGloves : NeutralItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingDuelistGloves = true;
            player.autoReuseGlove = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FeralClaws, 1).
                AddIngredient(ItemID.Cactus, 75).
                AddIngredient<RuntyBar>(8).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}