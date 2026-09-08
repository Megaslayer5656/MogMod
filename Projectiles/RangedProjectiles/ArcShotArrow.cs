using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    // arrow thats slow but explodes into a constant dragon piercer arrow rain for like 5 seconds
    // 28x76
    public class ArcShotArrow : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public ref float Timer => ref Projectile.ai[0];
        public float ShootTime = 120f;
        public bool SpawningArrows = false;
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 76;

            Projectile.arrow = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 2;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Timer++;
            Projectile.timeLeft = 2;
            Projectile.velocity *= MathHelper.Lerp(1.1f, 0.95f, Timer / 100);
            Main.NewText($"timer = {Timer}, vel = {MathHelper.Lerp(1.1f, 0.95f, Timer / 100)}");
    
            if (Timer >= ShootTime)
            {
                SpawningArrows = true;
                Projectile.velocity = Vector2.Zero;
                var source = Projectile.GetSource_FromThis();
                if (Timer % 2 == 0) MogModUtils.ProjectileRain(source, Projectile.Center, 400f, 50f, 1500f, 1700f, 50, ModContent.ProjectileType<DragonPiercerArrow>(), (int)(Projectile.damage * 0.25), Projectile.knockBack, Projectile.owner);
                if (Timer >= ShootTime * 5f) Projectile.Kill();
            }

            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10), DustID.Stone);
                dust.scale = Main.rand.NextFloat(0.3f, 0.7f);
                dust.velocity = -Projectile.velocity * 0.7f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Timer = ShootTime;
        public override bool? CanDamage() => !SpawningArrows;
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dustVelocity = new(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                dustVelocity.Normalize();
                dustVelocity *= 50;

                int dagonDust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Stone, 0, 0, 100, default, 1f);
                Dust dust = Main.dust[dagonDust];
                dust.noGravity = true;
                dust.position.X = Projectile.Center.X;
                dust.position.Y = Projectile.Center.Y;
                dust.position.X += (float)Main.rand.Next(-10, 11);
                dust.position.Y += (float)Main.rand.Next(-10, 11);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (SpawningArrows) return false;
            return true;
        }
    }
}