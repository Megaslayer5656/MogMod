using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class GlintstoneArcProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3());
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Terragrim, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.AncientLight, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, Color.LightBlue);
            }
            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha = (int)Utils.Remap(Projectile.timeLeft, 0, 60, 255, 0);
                Projectile.velocity *= 0.94f;
            }
            else if (Projectile.scale < 2f)
            {
                Projectile.velocity *= 0.99f;
                Projectile.scale += 0.005f;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Projectile.RotatingHitboxCollision(targetHitbox);
        public override bool? CanDamage() => (Projectile.alpha == 0 ? null : false);
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 2);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity *= 1.1f;

            if (Projectile.numHits >= 3 && Projectile.timeLeft > 60)
                Projectile.timeLeft = 60;
        }
    }
}