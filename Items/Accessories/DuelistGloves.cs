using MogMod.Common.MogModPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class DuelistGloves : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.Green;
        }
        // DOES WORK !! ! ! !
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
                AddIngredient(ItemID.SharkFin, 3).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
