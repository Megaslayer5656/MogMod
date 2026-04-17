using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class BouncyBoulderBulletProj : ModProjectile, ILocalizedModType
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
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.aiStyle = Terraria.ID.ProjAIStyleID.Boulder;
        }
        public override void AI()
        {            
            // makes it heavy
            Projectile.velocity.Y = Projectile.velocity.Y + 0.20f;
            if (Projectile.velocity.Y > 30f)
                Projectile.velocity.Y = 30f;

            // hurt player after some time (projectile frags friendlys)
            if (Projectile.timeLeft < 580)
                Projectile.hostile = true;

            // dust effect
            for (int k = 0; k < 1; k++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.3f);
                Main.dust[d].position = Projectile.Center;
                Main.dust[d].noLight = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Dazed, 240);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Dazed, 240);
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // this makes the projectile bounce once

            if (Projectile.ai[0] == 1)
            {
                // how many times it can bounce

                // increase the float value for more bounces
                if (Projectile.ai[1] < 30f)
                {
                    SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
                    if (Projectile.velocity.X != oldVelocity.X)
                        Projectile.velocity.X = -oldVelocity.X;

                    // bounce height
                    if (Projectile.velocity.Y != oldVelocity.Y)
                        Projectile.velocity.Y = -oldVelocity.Y;
                    Projectile.ai[1] += 1f;
                }
                else
                {
                    if (Projectile.velocity.X != oldVelocity.X)
                        Projectile.Kill();
                }
            }
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            int dustsplash = 0;
            while (dustsplash < 10)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PinkSlime, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 1f);
                Main.dust[d].position = Projectile.Center;
                dustsplash += 1;
            }
        }
    }
}