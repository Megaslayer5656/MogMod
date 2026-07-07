using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class CannonOfHaimaBoom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";

        private const float radius = 180f;

        public override void SetDefaults()
        {
            Projectile.width = 500;
            Projectile.height = 500;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            int numb = 3;
            if (Projectile.timeLeft >= 8)
            {
                for (int i = 0; i < 15; i++)
                {

                    Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    dustVelocity.Normalize();
                    dustVelocity *= 50;

                    int dagonDust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.HallowSpray, 0, 0, 100, default, 2.5f);
                    Dust dust = Main.dust[dagonDust];
                    dust.noGravity = true;
                    dust.position.X = Projectile.Center.X;
                    dust.position.Y = Projectile.Center.Y;
                    dust.position.X += (float)Main.rand.Next(-Projectile.width / numb, Projectile.width / numb);
                    dust.position.Y += (float)Main.rand.Next(-Projectile.height / numb, Projectile.height / numb);
                }
                for (int i = 0; i < 13; i++)
                {

                    Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    dustVelocity.Normalize();
                    dustVelocity *= 50;

                    int dagonDust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Electric, 0, 0, 100, default, 2.3f);
                    Dust dust = Main.dust[dagonDust];
                    dust.noGravity = true;
                    dust.position.X = Projectile.Center.X;
                    dust.position.Y = Projectile.Center.Y;
                    dust.position.X += (float)Main.rand.Next(-Projectile.width / numb, Projectile.width / numb);
                    dust.position.Y += (float)Main.rand.Next(-Projectile.height / numb, Projectile.height / numb);
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => MogModUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);
    }
}