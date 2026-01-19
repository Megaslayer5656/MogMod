using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class DagonThreeProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.RedTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            if (Projectile.velocity.X != Projectile.velocity.X)
            {
                Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                Projectile.velocity.X = -Projectile.velocity.X;
            }
            if (Projectile.velocity.Y != Projectile.velocity.Y)
            {
                Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                Projectile.velocity.Y = -Projectile.velocity.Y;
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 projPos = Projectile.position;
                    projPos -= Projectile.velocity * ((float)i * 0.25f);
                    Projectile.alpha = 255;
                    int dagonDust = Dust.NewDust(projPos, 1, 1, DustID.Flare, 0f, 0f, 0, Color.DarkRed, 0.75f);
                    Main.dust[dagonDust].noGravity = true;
                    Main.dust[dagonDust].position = projPos;
                    Main.dust[dagonDust].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[dagonDust].velocity *= 0.2f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 240);
        }

        public override void OnKill(int timeLeft)
        {
            int explosionDamage = Projectile.damage;
            float explosionKB = 6f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DagonExplosion>(), Convert.ToInt32(explosionDamage * .8), explosionKB, Projectile.owner);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Flare, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}