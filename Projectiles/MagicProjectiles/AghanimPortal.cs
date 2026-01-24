using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Mono.Cecil;
using System;
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
            Projectile.rotation += 0.1f;
            if (!initialized)
            {
                SoundEngine.PlaySound(SoundID.Item117, Projectile.Center);
                initialized = true;
            }

            // drift to a stop after being launched
            if (Projectile.timeLeft < 580)
                Projectile.velocity *= 0.882f;
            var source = Projectile.GetSource_FromThis();
            if ((Projectile.timeLeft == 550))
            {
                Projectile.NewProjectile(source, Projectile.Center, Projectile.velocity, ModContent.ProjectileType<AghanimLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 4; i++)
                {
                    int purpleDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmethyst);
                    Main.dust[purpleDust].noGravity = true;
                    Main.dust[purpleDust].scale = 1.75f;
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
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);
        }
    }
}