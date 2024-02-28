using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items
{
    public class MageShotgun : ModItem
    {
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.MogMod.hjson file.

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = 5;
            Item.knockBack = 6.5f;
            Item.value = 10000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item7;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.LostSoulFriendly;
            Item.shootSpeed = 10f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Emerald, 15);
            recipe.AddIngredient(ItemID.PalladiumBar, 15);
            recipe.AddIngredient(ItemID.CursedFlame, 20);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.Emerald, 15);
            recipe2.AddIngredient(ItemID.CobaltBar, 15);
            recipe2.AddIngredient(ItemID.CursedFlame, 20);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 7; i++) //replace 3 with however many projectiles you like

            {
                Random r = new Random();
                float PosX = player.position.X; //Makes the projectile always spawn above the cursor
                float PosY = player.position.Y; //makes the projectile spawn in the sky so it can shoot down
                float PosX2 = Main.MouseWorld.X - r.Next(100);
                float PosY2 = player.position.Y - r.Next(600, 800);
                Projectile.NewProjectile(source, PosX, PosY, 15f, 1f, type, 70, 8f, player.whoAmI); //create the projectile
                Projectile.NewProjectile(source, PosX, PosY, 15f, 1f, ProjectileID.LostSoulFriendly, 35, 5f, player.whoAmI);
            }
            return false;
        }
    }
}