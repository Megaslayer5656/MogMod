using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class IronBranch : ModItem
    {
        public override void SetDefaults()
        {
            Item.height = 50;
            Item.width = 32;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup(RecipeGroupID.Wood, 15).
                AddRecipeGroup(RecipeGroupID.IronBar, 3).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
