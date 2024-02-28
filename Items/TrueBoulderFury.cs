﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Items
{
    public class TrueBoulderFury : ModItem
    {
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.MogMod.hjson file.

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.width = 80;
            Item.height = 80;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = 1;
            Item.knockBack = 20f;
            Item.value = 100000;
            Item.rare = 6;
            Item.UseSound = SoundID.Item70;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Boulder;
            Item.shootSpeed = 30f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod, "BouncerFury");
            recipe.AddIngredient(ItemID.Boulder, 50);
            recipe.AddIngredient(ItemID.BrokenHeroSword, 1);
            recipe.AddIngredient(ItemID.SoulofFright, 15);
            recipe.AddIngredient(ItemID.SoulofMight, 15);
            recipe.AddIngredient(ItemID.SoulofSight, 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 7; i++) //replace 3 with however many projectiles you like

            {
                Random r = new Random();
                float PosX = Main.MouseWorld.X; //Makes the projectile always spawn above the cursor
                float PosY = player.position.Y - 600f; //makes the projectile spawn in the sky so it can shoot down
                float PosX2 = Main.MouseWorld.X - r.Next(100);
                float PosY2 = player.position.Y - r.Next(600, 800);
                Projectile.NewProjectile(source, PosX, PosY, 0f, 1f, type, 70, 8f, player.whoAmI); //create the projectile
                Projectile.NewProjectile(source, PosX2 - -50, PosY2, 0f, 1f, ProjectileID.MiniBoulder, 35, 5f, player.whoAmI);
            }
            return false;
        }
        }
    }