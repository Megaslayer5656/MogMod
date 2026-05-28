using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.EnemyProjectiles
{
    public class WarlockBoom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.EnemyProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 500;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Generic;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Burning, 300);
        public override bool? CanDamage()
        {
            if (Projectile.timeLeft > 0)
                return false;
            else
                return true;
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);
            int size = 540;
            for (int i = 0; i < 70; i++)
            {
                int dust = Dust.NewDust(Projectile.position, size, size, DustID.Flare, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 0, default, Main.rand.NextFloat(.8f, 1.6f));
                Main.dust[dust].velocity *= 1.4f;
            }
            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(Projectile.position, size, size, DustID.Smoke, 0f, 0f, 100, default, 1.2f);
                Main.dust[dust].velocity *= 1.4f;
            }
            for (int i = 0; i < 50; i++)
            {
                int dust = Dust.NewDust(Projectile.position, size, size, DustID.Torch, 0f, 0f, 100, default, 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 5f;
                dust = Dust.NewDust(Projectile.position, size, size, DustID.Torch, 0f, 0f, 100, default, 1.1f);
                Main.dust[dust].velocity *= 3f;
            }
            Projectile.localAI[1] = -1f;
            Projectile.maxPenetrate = -1;
            Projectile.Damage();
        }
    }
}