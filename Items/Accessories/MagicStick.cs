using MogMod.Common.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using MogMod.Items.Other;

namespace MogMod.Items.Accessories
{
    public class MagicStick : ModItem
    {

        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.stickActive = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Wood, 25).
                AddIngredient(ItemID.Emerald, 1).
                AddIngredient(ItemID.FallenStar, 1).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
