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
    public class DivineRapier : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // only applies effect when ABOVE base full health NOT max health with buffs
            // >= is above base max health
            // <= is when below base max health
            // == is when AT base max health
            if (player.statLife >= (player.statLifeMax2 * 1))
            {
                player.GetDamage(DamageClass.Generic) += 0.4f;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ShinyStone, 1).
                AddIngredient(ItemID.BrokenHeroSword, 1).
                AddIngredient(ItemID.HallowedKey, 1).
                AddIngredient(ItemID.HallowedBar, 10).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
