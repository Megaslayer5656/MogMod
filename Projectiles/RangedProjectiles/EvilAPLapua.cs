using MogMod.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class EvilAPLapua : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
            Projectile.light = .5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.scale = .60f;

            AIType = ProjectileID.Bullet;
        }
        public override void AI()
        {
            Projectile.localAI[1] += 1f;
            if (Projectile.timeLeft < 170)
                Projectile.velocity *= 0.932f;

            if (Projectile.timeLeft < 140)
                Projectile.ai[0] = 1f;

            if (Projectile.ai[0] >= 1f)
            {
                MogModUtils.HomeInOnNPC(Projectile, true, 800f, 15f, 15f);
                Projectile.extraUpdates = 70;
            }

            if (Projectile.localAI[1] > 4f)
            {
                for (int k = 0; k < 1; k++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.RainbowMk2, Projectile.velocity, 100, Color.BlueViolet, 1f);
                    dust.noGravity = true;
                }
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] < 1f)
            {
                return false;
            }
            return null;
        }

        public override bool CanHitPvp(Player target) => Projectile.ai[0] < 1f;
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}
