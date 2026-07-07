using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class StarsOfRuinHomingProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            float maxSpeed = 6;
            float currentSpeed = Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y;
            if (currentSpeed < maxSpeed * maxSpeed)
            {
                Projectile.velocity *= 2.2f;
            }
            if (Projectile.timeLeft < 590)
            
            MogModUtils.HomeInOnNPC(Projectile, true, 850f, 13f, 10f);

            Dust dust2 = Dust.NewDustPerfect(Projectile.position, DustID.AncientLight, Projectile.velocity, 100, Color.DarkBlue, 1.87f);
            dust2.noGravity = true;
            dust2.scale = Main.rand.NextFloat(1.617f, 2.1f);
            dust2.velocity *= 0.1f;

            Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.AncientLight, Projectile.velocity, 100, Color.LightBlue, 1.2f);
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.9f, 1.217f);
            dust.velocity *= 0.1f;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f, 100, Color.DarkBlue, 1.2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity -= Projectile.oldVelocity * 0.3f;

                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire, 0f, 0f, 100, default, 1.2f);
                Dust dust3 = Main.dust[dust2];
                dust3.noGravity = true;
                dust3.velocity *= 1.2f;
                dust3.velocity -= Projectile.oldVelocity * 0.3f;
            }
        }
    }
}