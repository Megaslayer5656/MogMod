using MogMod.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class ArchbeastArrow : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public static readonly SoundStyle dpFire = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/dragonPiercerShot")
        {
            Volume = 1.1f,
            PitchVariance = .2f,
            MaxInstances = 2
        };
        private bool initialized = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 68;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (!initialized)
            {
                initialized = true;
                SoundEngine.PlaySound(dpFire, Projectile.Center);
            }
            if (Projectile.timeLeft < 100)
            {
                Projectile.tileCollide = true;
            }

            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(Projectile.position, Convert.ToInt32(Projectile.width * 1.5), Convert.ToInt32(Projectile.height * 1.5), 31, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.9f);
                Main.dust[d].position = Projectile.Center;
                Main.dust[d].noLight = false;
            }
            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * -12f;
            for (int i = 0; i <= 5; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.Flare, shootVelocity.RotatedByRandom(MathHelper.ToRadians(18f)) * Main.rand.NextFloat(0.2f, 1.2f), 0, default, Main.rand.NextFloat(1f, 2.3f));
                dust.position = Projectile.Center;
                dust.scale = 1.5f;
                dust.alpha = 100;
                dust.noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            int dustsplash = 0;
            while (dustsplash < 8)
            {
                int d = Dust.NewDust(Projectile.position, Convert.ToInt32(Projectile.width * 2), Convert.ToInt32(Projectile.height * 2), DustID.Stone, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 0.9f);
                Main.dust[d].position = Projectile.Center;
                dustsplash += 1;
            }
        }
    }
}
