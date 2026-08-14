using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace MogMod.Items.Accessories.Rigs
{
    public class IdeaRig : ChestRig
    {
        //public const int MagSize = 15;
        //public const int MagReload = 40;
        //public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MagSize, MagReload.FramesToSeconds());
        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.width = Item.height = 32;
            
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            MogPlayer mogPlayer = player.MogMod();
            //mogPlayer.maxShots = MagSize;
            //mogPlayer.reloadTime = MagReload;
        }
        /* Prapor sells it
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Silk, 25).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Chest"}").
                AddTile(TileID.Loom).
                Register();
        }
        */
    }
}