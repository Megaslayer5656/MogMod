using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class RuntyStaffProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            if (Main.rand.Next(0, 3) == 0)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Tungsten, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            Projectile.velocity.Y = Projectile.velocity.Y + 0.25f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 5f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 projPos = Projectile.position;
                    projPos -= Projectile.velocity * ((float)i * 0.25f);
                    Projectile.alpha = 255;
                    int junkmunt = Dust.NewDust(projPos, 1, 1, DustID.Tungsten, 0f, 0f, 0, default, 0.75f);
                    Main.dust[junkmunt].noGravity = true;
                    Main.dust[junkmunt].position = projPos;
                    Main.dust[junkmunt].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[junkmunt].velocity *= 0.2f;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.Kill();
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => Projectile.Kill();
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            Projectile.damage = (int)(Projectile.damage * 1.15);
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.Kill();
                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int k = 0; k < 5; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Tungsten, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
        }
    }
}