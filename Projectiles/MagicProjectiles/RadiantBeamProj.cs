using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class RadiantBeamProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 250;
            Projectile.MaxUpdates = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

        }
        public override void AI()
        {
            float pi = MathHelper.Pi;
            if (Projectile.timeLeft < 100)
            {
                Projectile.tileCollide = true;
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] == 48f)
            {
                Projectile.ai[0] = 0f;
            }
            else
            {
                for (int d = 0; d < 2; d++)
                {
                    Vector2 offset = Vector2.UnitY * -12f;
                    offset = -Vector2.UnitX.RotatedBy((double)(Projectile.ai[0] * pi / 24f + (float)d * pi), default) * new Vector2(10f, 5f) - Projectile.rotation.ToRotationVector2() * 10f;
                    Dust dust = Dust.NewDustPerfect(Projectile.position, 180, Projectile.velocity, 100, default, 1f);
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.9f, 1.4f);
                    dust.position = Projectile.Center + offset;
                    dust.velocity = Projectile.velocity;
                }
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 0f)
            {
                for (int d = 0; d < 4; d++)
                {
                    Vector2 source = Projectile.position;
                    source -= Projectile.velocity * ((float)d * 0.25f);
                    Dust dust = Dust.NewDustPerfect(Projectile.position, 206, Projectile.velocity, 100, default, 1f);
                    dust.noGravity = true;
                    dust.position = source;
                    dust.scale = Main.rand.NextFloat(1.2f, 1.7f);
                    dust.velocity *= 0.1f;
                }
            }
            MogModUtils.HomeInOnNPC(Projectile, true, 350f, 13f, 10f);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 180, 0f, 0f, 100, default, 1.2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity -= Projectile.oldVelocity * 0.3f;

                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 206, 0f, 0f, 100, default, 1.2f);
                Dust dust3 = Main.dust[dust2];
                dust3.noGravity = true;
                dust3.velocity *= 1.2f;
                dust3.velocity -= Projectile.oldVelocity * 0.3f;
            }
        }
    }
}