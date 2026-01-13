using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class ShrapnalProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
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
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
            Projectile.light = 1f;
            Projectile.extraUpdates = 1;
            Projectile.scale = .60f;

            AIType = ProjectileID.Bullet;
        }
        public override void AI()
        {
            if (Main.rand.NextBool(25))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Copper, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.9f);
                Main.dust[d].position = Projectile.Center;
                Main.dust[d].noLight = true;
            }
        }
        public override void OnKill(int timeLeft)
        {
            var source = Projectile.GetSource_FromThis();
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            for (int n = 0; n < 8; n++)
            {
                MogModUtils.ProjectileRain(source, Projectile.Center, 400f, 50f, 1500f, 1500f, 25, ModContent.ProjectileType<ShrapnalSkyProj>(), Convert.ToInt32(Projectile.damage * .25), Projectile.knockBack, Projectile.owner);
            }
        }
    }
}