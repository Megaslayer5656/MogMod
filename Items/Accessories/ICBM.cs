using MogMod.Common.MogModPlayer;
using MogMod.Items.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace MogMod.Items.Accessories
{
    public class ICBM : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Expert;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Generic) += .075f;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.atgActive = true;
            mogPlayer.icbmActive = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
            AddIngredient(ItemID.LunarBar, 15).
            AddIngredient<ATGMissile>(1).
            AddIngredient(ItemID.RocketIII, 10).
            AddTile(TileID.MythrilAnvil).
            Register();
        }
    }
}
