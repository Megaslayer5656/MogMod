using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class Greafer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public int shotFired = 0;
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 32;

            Item.damage = 108;
            Item.knockBack = 5f;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.useTime = 5;
            Item.useAnimation = 20;
            Item.reuseDelay = 18;
            Item.useLimitPerAnimation = 4;
            Item.consumeAmmoOnFirstShotOnly = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = SoundID.Item41;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float Spread = 0.05f;
            SoundEngine.PlaySound(SoundID.Item41, player.Center);
            switch (shotFired)
            {
                case 0:
                    Projectile deathShot = Projectile.NewProjectileDirect(source, position + velocity.RotatedBy(-0.6 * player.direction) - velocity * 0.5f, velocity.RotatedBy(Spread * 2f), type, damage, knockback, player.whoAmI);
                    MogModGlobalProjectile mogProj = deathShot.MogMod();
                    mogProj.deathBullet = true;
                    shotFired++;
                    break;
                case 1:
                    Projectile daybreakShot = Projectile.NewProjectileDirect(source, position + velocity.RotatedBy(-0.6 * player.direction) - velocity * 0.5f, velocity.RotatedBy(Spread), type, damage, knockback, player.whoAmI);
                    MogModGlobalProjectile mogProj2 = daybreakShot.MogMod();
                    mogProj2.daybreakBullet = true;
                    shotFired++;
                    break;
                case 2:
                    Projectile deathShot2 = Projectile.NewProjectileDirect(source, position + velocity.RotatedBy(-0.6 * player.direction) - velocity * 0.5f, velocity.RotatedBy(-Spread), type, damage, knockback, player.whoAmI);
                    MogModGlobalProjectile mogProj3 = deathShot2.MogMod();
                    mogProj3.deathBullet = true;
                    shotFired++;
                    break;
                case 3:
                    Projectile daybreakShot2 = Projectile.NewProjectileDirect(source, position + velocity.RotatedBy(-0.6 * player.direction) - velocity * 0.5f, velocity.RotatedBy(-Spread * 2f), type, damage, knockback, player.whoAmI);
                    MogModGlobalProjectile mogProj4 = daybreakShot2.MogMod();
                    mogProj4.daybreakBullet = true;
                    shotFired = 0;
                    break;
            }
            return false;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-4f, 0f);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Jericolt>().
                AddIngredient<Tripshot>().
                AddIngredient<GriefBar>(8).
                AddIngredient<LizhardBloodVial>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}