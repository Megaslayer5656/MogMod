using Microsoft.Xna.Framework;
using MogMod.Projectiles.MagicProjectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.ClasslessProjectiles
{
    public class UndyingPortalProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.ClasslessProjectiles";
        private bool initialized = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 350;
            Projectile.DamageType = DamageClass.Generic;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Projectile.rotation += 0.1f;
            if (!initialized)
            {
                SoundEngine.PlaySound(SoundID.Item117, Projectile.Center);
                initialized = true;
            }

            // drift to a stop after being launched
            if (Projectile.timeLeft < 330)
                Projectile.velocity *= 0.882f;
            var source = Projectile.GetSource_FromThis();
            if (Projectile.timeLeft <= 300 && Projectile.timeLeft % 60 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);

                Random spawnNumb = new Random();
                int[] amount = { 4, 6, 8 };
                int choice = amount[spawnNumb.Next(amount.Length)];

                float offset = Main.rand.NextFloat(MathHelper.TwoPi);
                for (int i = 0; i < choice; i++)
                {
                    Vector2 velocity = ((MathHelper.TwoPi * i / choice) - offset).ToRotationVector2() * (choice / 2);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<UndyingHomingProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 4; i++)
                {
                    int deathDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemEmerald);
                    Main.dust[deathDust].noGravity = true;
                    Main.dust[deathDust].scale = 1.75f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 70)
            {
                float timeAlpha = (float)Projectile.timeLeft / 10f;
                Projectile.alpha = (int)(255f - 255f * timeAlpha);
            }
            return new Color(255 - Projectile.alpha, 153 - Projectile.alpha, 204 - Projectile.alpha, 0);
        }
    }
}