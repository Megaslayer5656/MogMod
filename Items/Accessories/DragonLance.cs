using MogMod.Items.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class DragonLance : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.LightRed;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Ranged) += .10f;
            player.GetDamage(DamageClass.Ranged) += .07f;
            Player.tileRangeX = Player.tileRangeY += 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BeltOfStrength>(1).
                AddIngredient(ItemID.GreenPhaseblade, 1).
                AddRecipeGroup("TissueSample", 7).
                AddIngredient(ItemID.Ruby, 5).
                AddIngredient(ItemID.AntlionMandible, 3).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
