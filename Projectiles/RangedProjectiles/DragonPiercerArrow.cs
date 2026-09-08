using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class DragonPiercerArrow : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        Projectile potentialTarget = null;
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 64;

            Projectile.arrow = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.timeLeft = 119;
            Projectile.MaxUpdates = 2;
        }
        private Vector2 Recalibrate()
        {
            float turnSpeedFactor = (float)Math.Pow(MathHelper.Clamp(Projectile.timeLeft - 40, 0f, 120f) / 120f, 4D);
            float turnAngle = MathHelper.ToRadians(turnSpeedFactor * 75f);

            Vector2 leftTurnVelocity = Projectile.velocity.RotatedBy(-turnAngle);
            Vector2 righTurnVelocity = Projectile.velocity.RotatedBy(turnAngle);
            float leftDirectionImprecision = leftTurnVelocity.AngleBetween(Projectile.SafeDirectionTo(potentialTarget.Center));
            float rightDirectionImprecision = righTurnVelocity.AngleBetween(Projectile.SafeDirectionTo(potentialTarget.Center));

            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type == ModContent.ProjectileType<TracerArrow>())
                    potentialTarget = proj;
            }

            if (leftDirectionImprecision < rightDirectionImprecision) return leftTurnVelocity;
            else return righTurnVelocity;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile.tileCollide = Projectile.localAI[1]++ > 30f;

            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type == ModContent.ProjectileType<TracerArrow>())
                    potentialTarget = proj;
            }

            if (potentialTarget != null)
            {
                float angularTurnSpeed = MathHelper.ToRadians(2.5f);
                float idealDirection = Projectile.AngleTo(potentialTarget.Center);
                float updatedDirection = Projectile.velocity.ToRotation().AngleTowards(idealDirection, angularTurnSpeed);
                Projectile.velocity = updatedDirection.ToRotationVector2() * Projectile.velocity.Length();

                if (Projectile.timeLeft % 6 == 0)
                {
                    Projectile.velocity = Recalibrate();
                }
            }
        }
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
    }
}