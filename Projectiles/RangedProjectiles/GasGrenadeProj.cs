using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public sealed class GasGrenadeProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "MogMod/Items/Weapons/Ranged/GasGrenade";
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 16;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.aiStyle = ProjAIStyleID.GroundProjectile;
            Projectile.timeLeft = 180;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);
            float offset = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < Main.rand.Next(4, 8); i++)
            {
                float value = Main.rand.NextFloat(1f, 3f);
                Vector2 velocity = ((MathHelper.TwoPi * i / (value * 2)) - offset).ToRotationVector2() * 2f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<GasExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            for (int i = 0; i < 20; i++)
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.FartInAJar, 0f, 0f, 0, default(Color), 1f);
            for (int i = 0; i < 10; i++)
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 0, default(Color), 1f);
        }
    }
}