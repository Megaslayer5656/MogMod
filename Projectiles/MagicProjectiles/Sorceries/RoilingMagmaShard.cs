using Microsoft.Xna.Framework;
using MogMod.Common.Classes;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class RoilingMagmaShard : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public ref float Time => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.timeLeft = 600;
        }
        public override void AI()
        {
            Time++;
            if (Time < 10)
                Projectile.tileCollide = false;
            else
                Projectile.tileCollide = true;
            float rotateratio = 0.019f;
            float rotation = (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * rotateratio;
            Projectile.rotation += rotation * Projectile.direction;
            Projectile.velocity.Y = Projectile.velocity.Y + 0.25f;
            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;
            Dust fDust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? DustID.Lava : DustID.Flare, Projectile.velocity, 100, default, 1.2f);
            fDust.noGravity = true;
            fDust.velocity *= 0.1f;
            if (Time >= 8f)
            {
                float flameDustSize = Utils.GetLerpValue(6f, 12f, Time, true);
                Dust flameDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? DustID.Flare : DustID.Lava, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 10, default, 0.25f);
                if (Main.rand.NextBool(3))
                {
                    flameDust.scale *= 3f;
                    flameDust.velocity *= 1.5f;
                }
                flameDust.noGravity = true;
                flameDust.scale *= flameDustSize * 0.8f;
                flameDust.velocity += Projectile.velocity;
                int fireDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 25, 0f, 0f, 200, default, 0.2f);
                Dust dust = Main.dust[fireDust];
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)Projectile.width / 8f;
                dust.noGravity = true;
                dust.velocity *= 3f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire, 180);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.OnFire, 180);
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            for (int k = 0; k < 15; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, Main.rand.NextBool() ? DustID.Flare : DustID.Lava, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.25f);
            for (int i = 0; i < 9; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 25, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, 100, default, 0.5f);
                Main.dust[dust].velocity *= 1.4f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}