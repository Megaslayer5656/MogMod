using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using MogMod.Buffs.Debuffs;
using Terraria.DataStructures;
using Terraria.Audio;

namespace MogMod.Projectiles.EnemyProjectiles.Boss
{
    public class VonGreenTracerProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.BossProjectiles";
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
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.light = .5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;

            AIType = ProjectileID.Bullet;
        }
        public override void AI()
        {
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.TerraBlade, Projectile.velocity, 100, default, 0.5f);
                dust.noGravity = true;
                dust.noLight = true;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
             Projectile.Kill();
                return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<VonDebuff>(), 240);
        }
    }
}