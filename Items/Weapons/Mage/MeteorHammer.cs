using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Projectiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Mage
{
    public class MeteorHammer : ModItem
    {
        // The base use time of the weapon is 47: 28 charge frames + 19 cooldown frames.
        // The rate at which it progresses through its charge and discharge cycle is dynamically sped up by reforges.
        // This math is handled in its holdout projectile, MeteorHammerHoldout.
        public const int ChargeFrames = 28;
        public const int CooldownFrames = 19;
        public const float GemDistance = 18f;
        public static readonly Color LightColor = new Color(235, 40, 121);

        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;
            Item.damage = 1250;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 64;
            Item.useTime = Item.useAnimation = 80;
            Item.rare = ItemRarityID.Red;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 15f;
            Item.UseSound = SoundID.Item88;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MeteorHammerProjectile>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float meteorSpeed = Item.shootSpeed;
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float meteorSpawnXPos = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
            float meteorSpawnYPos = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
            if (player.gravDir == -1f)
            {
                meteorSpawnYPos = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - realPlayerPos.Y;
            }
            float meteorSpawnDist = (float)Math.Sqrt((double)(meteorSpawnXPos * meteorSpawnXPos + meteorSpawnYPos * meteorSpawnYPos));
            if ((float.IsNaN(meteorSpawnXPos) && float.IsNaN(meteorSpawnYPos)) || (meteorSpawnXPos == 0f && meteorSpawnYPos == 0f))
            {
                meteorSpawnXPos = (float)player.direction;
                meteorSpawnYPos = 00f;
                meteorSpawnDist = meteorSpeed;
            }
            else
            {
                meteorSpawnDist = meteorSpeed / meteorSpawnDist;
            }

            int asteroidAmt = 1;
            for (int i = 0; i < asteroidAmt; i++)
            {
                realPlayerPos = new Vector2(player.Center.X + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                realPlayerPos.X = (realPlayerPos.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                realPlayerPos.Y -= (float)(100 * i);
                meteorSpawnXPos = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X + (float)Main.rand.Next(-40, 41) * 0.03f;
                meteorSpawnYPos = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
                if (meteorSpawnYPos < 0f)
                {
                    meteorSpawnYPos *= -1f;
                }
                if (meteorSpawnYPos < 20f)
                {
                    meteorSpawnYPos = 20f;
                }
                meteorSpawnDist = (float)Math.Sqrt((double)(meteorSpawnXPos * meteorSpawnXPos + meteorSpawnYPos * meteorSpawnYPos));
                meteorSpawnDist = meteorSpeed / meteorSpawnDist;
                meteorSpawnXPos *= meteorSpawnDist;
                meteorSpawnYPos *= meteorSpawnDist;
                float meteorSpawnXOffset = meteorSpawnXPos;
                float meteorSpawnYOffset = meteorSpawnYPos + (float)Main.rand.Next(-40, 41) * 0.02f;
                Projectile.NewProjectile(source, realPlayerPos.X, (realPlayerPos.Y / 1.25f), meteorSpawnXOffset * 0.75f, meteorSpawnYOffset * 0.75f, type, damage, knockback, player.whoAmI, 0f, 0.5f + (float)Main.rand.NextDouble() * 0.3f);
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Kaya>(1).
                AddIngredient(ItemID.MeteorStaff, 1).
                AddIngredient(ItemID.StaffofEarth, 1).
                AddIngredient(ItemID.FragmentVortex, 8).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}