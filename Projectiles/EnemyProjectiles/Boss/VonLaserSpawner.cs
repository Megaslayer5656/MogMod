using Microsoft.Xna.Framework;
using MogMod.Projectiles.MagicProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.EnemyProjectiles.Boss
{
    public class VonLaserSpawner : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.BossProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.timeLeft = 50;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Generic;
        }
        public override void AI()
        {
            Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.AncientLight, Projectile.velocity, 100, Color.DarkRed, 1f);
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.91f, 1.417f);
            dust.velocity *= 0.1f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 4f;
                Projectile.NewProjectile(source, Projectile.Center, velocity, ModContent.ProjectileType<VonTargetLaser>(), 0, 0, Projectile.owner);
                SoundEngine.PlaySound(SoundID.Item15, Projectile.Center);
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 4f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<VonLaserEyes>(), Projectile.damage, 0, Projectile.owner);
                SoundEngine.PlaySound(SoundID.Item68, Projectile.Center);
            }
        }
        public override bool? CanDamage() => false;
    }
}
