using Microsoft.Xna.Framework;
using MogMod.Projectiles.MagicProjectiles;
using MogMod.Utilities;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class IsraelArrowProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.arrow = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Main.rand.NextBool(3))
            {
            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * -12f;
                int d = Dust.NewDust(Projectile.position, (int)(Projectile.width * 1.5f), (int)(Projectile.height * 1.5f), DustID.PlatinumCoin, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.9f);
                Main.dust[d].position = Projectile.Center;
                Main.dust[d].noLight = false;
                Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.GoldCoin, shootVelocity.RotatedByRandom(MathHelper.ToRadians(18f)) * Main.rand.NextFloat(0.2f, 1.2f), 0, default, Main.rand.NextFloat(1f, 2.3f));
                dust.position = Projectile.Center;
                dust.scale = 1.5f;
                dust.alpha = 100;
                dust.noGravity = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Midas, 600);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Midas, 600);
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            MogModUtils.ProjectileRain(Projectile.GetSource_FromThis(), Projectile.Center, 0f, 0f, -10f, -10f, 10f, ModContent.ProjectileType<GoyBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            int dustsplash = 0;
            while (dustsplash < 8)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width * 2, Projectile.height * 2, DustID.Stone, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 0.9f);
                Main.dust[d].position = Projectile.Center;
                dustsplash += 1;
            }
        }
    }
}