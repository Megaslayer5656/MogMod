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
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        NPC potentialTarget = null;
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.timeLeft = 119;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 2;
            Projectile.DamageType = DamageClass.Ranged;
        }

        private Vector2 Recalibrate()
        {
            float turnSpeedFactor = (float)Math.Pow(MathHelper.Clamp(Projectile.timeLeft - 40, 0f, 120f) / 120f, 4D);
            float turnAngle = MathHelper.ToRadians(turnSpeedFactor * 75f);

            Vector2 leftTurnVelocity = Projectile.velocity.RotatedBy(-turnAngle);
            Vector2 righTurnVelocity = Projectile.velocity.RotatedBy(turnAngle);
            float leftDirectionImprecision = leftTurnVelocity.AngleBetween(Projectile.SafeDirectionTo(potentialTarget.Center));
            float rightDirectionImprecision = righTurnVelocity.AngleBetween(Projectile.SafeDirectionTo(potentialTarget.Center));
            potentialTarget = Projectile.Center.ClosestNPCAt(512f, true);

            if (leftDirectionImprecision < rightDirectionImprecision)
                return leftTurnVelocity;
            else
                return righTurnVelocity;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (potentialTarget == null)
                potentialTarget = Projectile.Center.ClosestNPCAt(512f, true);

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

                Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
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
