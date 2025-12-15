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
    public class Diadem : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Summon) += .07f;
            player.maxTurrets += 1;
            player.maxMinions += 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.GoldBar, 20).
                AddIngredient(ItemID.ManaCrystal, 5).
                AddIngredient(ItemID.Sapphire, 7).
                AddIngredient(ItemID.CrimtaneBar, 3).
                AddTile(TileID.Anvils).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.GoldBar, 20).
                AddIngredient(ItemID.ManaCrystal, 5).
                AddIngredient(ItemID.Sapphire, 7).
                AddIngredient(ItemID.DemoniteBar, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
