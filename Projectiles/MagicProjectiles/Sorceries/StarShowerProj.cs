using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class StarShowerProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.timeLeft = 50;
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
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item43, Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f, 100, Color.LightBlue, .8f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity -= Projectile.oldVelocity * 0.3f;

                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ManaRegeneration, 0f, 0f, 100, default, .8f);
                Dust dust3 = Main.dust[dust2];
                dust3.noGravity = true;
                dust3.velocity *= 1.2f;
                dust3.velocity -= Projectile.oldVelocity * 0.3f;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                SummonExtraProj();
            }
        }
        public override bool? CanDamage() => false;
        private void SummonExtraProj()
        {
            float offset = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < 6; i++)
            {
                Vector2 velocity = ((MathHelper.TwoPi * i / 6f) - offset).ToRotationVector2() * 3f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<StarShowerHomingProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }
}