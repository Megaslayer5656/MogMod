using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class StarsOfRuinProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        int numb = 0;
        float offset = Main.rand.NextFloat(MathHelper.TwoPi);
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.timeLeft = 80;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
        }
        public override void AI()
        {
            Projectile.velocity *= 0.95f;

            Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.AncientLight, Projectile.velocity, 100, Color.LightBlue, 1f);
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.91f, 1.417f);
            dust.velocity *= 0.1f;

            if (Projectile.timeLeft <= 60 && Projectile.timeLeft % 5 == 0)
            {
                numb++;
                SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                Vector2 velocity = ((MathHelper.TwoPi * numb / 6f) - offset).ToRotationVector2() * 3f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<StarsOfRuinHomingProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 7; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f, 100, Color.LightBlue, .8f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity -= Projectile.oldVelocity * 0.3f;

                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire, 0f, 0f, 100, default, .8f);
                Dust dust3 = Main.dust[dust2];
                dust3.noGravity = true;
                dust3.velocity *= 1.2f;
                dust3.velocity -= Projectile.oldVelocity * 0.3f;
            }
        }
        public override bool? CanDamage() => false;
    }
}