using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class BoulderBulletProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;

            AIType = ProjectileID.Bullet;
        }
        public override void AI()
        {
            //Code to make it not shoot backwards
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            //Gravity
            Projectile.velocity.Y = Projectile.velocity.Y + 0.20f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
            if (Projectile.timeLeft < 595)
                Projectile.tileCollide = true;

            if (Projectile.timeLeft < 570)
            {
                Projectile.hostile = true;
            }

            for (int k = 0; k < 1; k++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.3f);
                Main.dust[d].position = Projectile.Center;
                Main.dust[d].noLight = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Dazed, 180);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Dazed, 180);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}
