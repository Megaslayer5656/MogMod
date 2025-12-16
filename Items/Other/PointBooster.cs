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
    public class PointBooster : ModItem
    {
        public override void SetDefaults()
        {
            Item.height = 50;
            Item.width = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Pink;
            Item.value = 8732;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.OrichalcumBar, 7).
                AddIngredient(ItemID.SoulofLight, 5).
                AddIngredient(ItemID.ManaCrystal, 3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
