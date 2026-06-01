using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class RuntyFlamethrowerProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public ref float Time => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3;
            Projectile.MaxUpdates = 2;
            Projectile.timeLeft = 42 * Projectile.MaxUpdates;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            // Kill the fire when the water
            if (Projectile.wet && !Projectile.lavaWet)
            {
                Projectile.Kill();
                return;
            }
            Time++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Time >= 8f)
            {
                float cinderSize = Utils.GetLerpValue(6f, 12f, Time, true);
                Dust cinder = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 10, default, 0.75f);
                if (Main.rand.NextBool(3))
                {
                    cinder.scale *= 3f;
                    cinder.velocity *= 1.5f;
                }
                cinder.noGravity = true;
                cinder.scale *= cinderSize * 0.8f;
                cinder.velocity += Projectile.velocity;
                int fireDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 200, default, 2.7f);
                Dust dust = Main.dust[fireDust];
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(5, 20+1));
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(5, 20+1));
    }
}