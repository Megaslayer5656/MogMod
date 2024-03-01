using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Social.WeGame;
namespace MogMod.Items.Weapons
{
    public class MageShotgun : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = 5;
            Item.knockBack = 6.5f;
            Item.value = 10000;
            Item.rare = 5;
            Item.UseSound = SoundID.Item88;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.LostSoulFriendly;
            Item.shootSpeed = 20f;
            Item.mana = 20;
            Item.noMelee = true;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 100;
                Item.useAnimation = 100;
                Item.UseSound = SoundID.Item66;
            }
            else
            {
                Item.useTime = 30;
                Item.useAnimation = 30;
                Item.UseSound = SoundID.Item88;
            }
            return true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.SpectreBar, 15);
            recipe.AddIngredient(ItemID.SpectreStaff, 1);
            recipe.AddIngredient(ItemID.Xenopopper, 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 3 + Main.rand.Next(3); // 3, 4, or 5 shots
            float rotation = MathHelper.ToRadians(15);
            if (player.altFunctionUse == 2)
            {
                Vector2 cvelocity = Vector2.Normalize(velocity) * 5f;
                int proj = Projectile.NewProjectile(source, position, cvelocity, ProjectileID.ChlorophyteOrb, 500, knockback, player.whoAmI);
                Main.projectile[proj].friendly = true;
                return false;
            }
            position += Vector2.Normalize(velocity) * 45f;
            float PosX = Main.MouseWorld.X;
            float PosY = player.position.Y;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }
                return false;
            }
        }
    }
