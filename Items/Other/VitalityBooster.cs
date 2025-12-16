using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace MogMod.Items.Other
{
    public class VitalityBooster : ModItem
    {
        public override void SetDefaults()
        {
            Item.height = 50;
            Item.width = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Pink;
            Item.value = 4000;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrimtaneBar, 5).
                AddIngredient(ItemID.LifeCrystal, 3).
                AddIngredient(ItemID.Diamond, 1).
                AddTile(TileID.Anvils).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.DemoniteBar, 5).
                AddIngredient(ItemID.LifeCrystal, 3).
                AddIngredient(ItemID.Diamond, 1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
