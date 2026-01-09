using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class DagonFiveOrb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        private bool initialized = false;
        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
            //Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = 100;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            if (!initialized)
            {
                initialized = true;
                float dustAmt = 12f;
                int d = 0;
                while ((float)d < dustAmt)
                {
                    Vector2 offset = Vector2.UnitX * 0f;
                    offset += -Vector2.UnitY.RotatedBy((double)((float)d * (MathHelper.TwoPi / dustAmt)), default) * new Vector2(1f, 6f);
                    offset = offset.RotatedBy((double)Projectile.velocity.ToRotation(), default);
                    int i = Dust.NewDust(Projectile.Center, 0, 0, DustID.SolarFlare, 0f, 0f, 0, default, 1f);
                    Main.dust[i].scale = 1.25f;
                    Main.dust[i].noGravity = true;
                    Main.dust[i].position = Projectile.Center + offset;
                    Main.dust[i].velocity = Projectile.velocity * 0f + offset.SafeNormalize(Vector2.UnitY) * 1f;
                    d++;
                }
            }
            Vector2 value7 = new Vector2(5f, 10f);
            Projectile.ai[1] += 1f;
            for (int dust = 0; dust < 2; dust++)
            {
                Vector2 dustPosOffset = Vector2.UnitX * -12f;
                dustPosOffset = -Vector2.UnitY.RotatedBy((double)(Projectile.ai[1] * 0.1308997f + (float)dust * 3.14159274f), default) * value7 - Projectile.rotation.ToRotationVector2() * 10f;
                int exo = Dust.NewDust(Projectile.Center, 0, 0, DustID.SolarFlare, 0f, 0f, 160, default, 1f);
                Main.dust[exo].scale = 1f;
                Main.dust[exo].noGravity = true;
                Main.dust[exo].position = Projectile.Center + dustPosOffset;
                Main.dust[exo].velocity = Projectile.velocity;
            }

            if (Projectile.timeLeft < 280)
                Projectile.ai[0] = 1f;

            if (Projectile.ai[0] >= 1f)
                MogModUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 500f, 15f, 15f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            int explosionDamage = Projectile.damage;
            float explosionKB = 6f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DagonFiveExplosion>(), Convert.ToInt32(explosionDamage * 0.85), explosionKB, Projectile.owner);
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.SolarFlare, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 600);
            //SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            //int explosionDamage = Projectile.damage;
            //float explosionKB = 6f;
            //Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DagonFiveExplosion>(), Convert.ToInt32(explosionDamage * 0.85), explosionKB, Projectile.owner);
            //for (int k = 0; k < 3; k++)
            //{
            //    Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.SolarFlare, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            //}
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Daybreak, 600);
            //SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            //int explosionDamage = Projectile.damage;
            //float explosionKB = 6f;
            //Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DagonFiveExplosion>(), Convert.ToInt32(explosionDamage * 0.85), explosionKB, Projectile.owner);
            //for (int k = 0; k < 3; k++)
            //{
            //    Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.SolarFlare, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            //}
        }

        // Cannot deal damage for the first several frames of existence.
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft >= 280)
            {
                return false;
            }
            return null;
        }

        public override bool CanHitPvp(Player target) => Projectile.timeLeft < 280;
    }
}