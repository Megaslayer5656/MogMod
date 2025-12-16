using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Items.Placeable;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Exceptions;
namespace MogMod.Items.Accessories
{
    public class DesperationCharm : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public static readonly int AdditiveDamageBonus = 100;
        public static readonly int AttackSpeedBonus = 25;
        public override void SetDefaults() 
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.statLife < player.statLifeMax2 * 0.25f)
                {
                player.GetDamage(DamageClass.Generic) += AdditiveDamageBonus / 100f;
                player.GetAttackSpeed(DamageClass.Generic) += AttackSpeedBonus / 100f;
            }
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<SoulOfDabdad>(5);
            recipe.AddIngredient<DabDadBar>(10);
            recipe.AddIngredient(ItemID.CharmofMyths, 1);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}