using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
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
        public float ShootTime = 180f;
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
            Projectile.tileCollide = Projectile.ai[0] != 1f && Projectile.localAI[1]++ > 20f;
            Projectile.velocity *= MathHelper.Lerp(1.03f, 0.975f, Timer / 100);
            Projectile.velocity.Y = Projectile.velocity.Y + 0.030f;
            if (Projectile.velocity.Y > 16f) Projectile.velocity.Y = 16f;

            if (Timer >= ShootTime)
            {
                if (!SpawningArrows)
                {
                    for (int i = 0; i < 25; i++)
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
                    SpawningArrows = true;
                    SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                }
                Projectile.velocity = Vector2.Zero;
                var source = Projectile.GetSource_FromThis();
                if (Timer % 5 == 0) MogModUtils.ProjectileRain(source, Projectile.Center, 400f, 50f, 1500f, 1700f, 40, ModContent.ProjectileType<DragonPiercerArrow>(), (int)(Projectile.damage * 0.2), Projectile.knockBack, Projectile.owner);
                if (Timer >= ShootTime * 5f) Projectile.Kill();
            }
            else
                if (Main.rand.NextBool(4))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10), DustID.Stone);
                    dust.scale = Main.rand.NextFloat(0.3f, 0.7f);
                    dust.velocity = -Projectile.velocity * 0.7f;
                }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Timer = ShootTime;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Timer = ShootTime;
            return false;
        }
        public override bool? CanDamage() => !SpawningArrows;
        public override bool PreDraw(ref Color lightColor)
        {
            if (SpawningArrows) return false;
            return true;
        }
    }
}