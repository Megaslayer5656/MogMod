using MogMod.Common.Player;
using MogMod.Items.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Transactions;

namespace MogMod.Items.Accessories
{
    public class ShadowAmulet : ModItem
    {
        public int i;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //TODO: Make it turn you invis after 5 seconds of standing still
            player.invis = true;
            player.statManaMax2 += 30;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Amethyst, 7).
                AddIngredient(ItemID.ManaCrystal, 1).
                AddIngredient(ItemID.InvisibilityPotion, 2).
                AddIngredient(ItemID.Sapphire, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
