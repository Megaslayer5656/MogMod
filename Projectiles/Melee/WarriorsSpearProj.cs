using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.Audio;

namespace MogMod.Projectiles.Melee
{
    public class WarriorsSpearProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.JavelinFriendly;

            Projectile.scale = 1.15f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[2] == 5f) target.AddBuff(BuffID.OnFire, 300);
        }
        public override void AI()
        {
            if (Projectile.ai[2] == 5f) if (Main.rand.NextBool(5))
            {
                for (int i = 0; i < 5; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.9f);
                    Main.dust[d].position = Projectile.Center;
                    Main.dust[d].noLight = false;
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}