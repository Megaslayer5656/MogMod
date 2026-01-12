using Microsoft.Xna.Framework;
using MogMod.Utilities;
using MonoMod.Core.Utils;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class AghanimPortal : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
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
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            if (!initialized)
            {
                SoundEngine.PlaySound(SoundID.Item117, Projectile.Center);
                initialized = true;
            }
                Projectile.rotation += 0.1f;
            // drift to a stop after being launched
            if (Projectile.timeLeft < 580)
                Projectile.velocity *= 0.882f;

            NPC target = Projectile.Center.ClosestNPCAt(800);
            if ((Projectile.timeLeft == 570) && target != null)
            {
                MogModUtils.MagnetSphereHitscan(Projectile, Vector2.Distance(Projectile.Center, target.Center), 8f, 0, 3, ModContent.ProjectileType<AghanimLaser>(), 1D, true);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 70)
            {
                float timeAlpha = (float)Projectile.timeLeft / 10f;
                Projectile.alpha = (int)(255f - 255f * timeAlpha);
            }
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);
        }
    }
}