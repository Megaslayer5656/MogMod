using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Projectiles;
using System;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class Yasha : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 66;
            Item.damage = 20;
            Item.knockBack = 5;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.consumable = false;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.value = Item.buyPrice(0, 50, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.shootSpeed = 8f;
            Item.shoot = ModContent.ProjectileType<YashaProjectile>();
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 46;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<YashaProjectile>();
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Javelin, 25).
                AddIngredient(ItemID.ChlorophyteBar, 15).
                AddIngredient(ItemID.SoulofSight, 7).
                AddIngredient(ItemID.SoulofFlight, 7).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}