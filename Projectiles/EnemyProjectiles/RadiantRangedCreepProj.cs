using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.EnemyProjectiles
{
    public class RadiantRangedCreepProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Enemy";
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            Dust d = Dust.NewDustPerfect(Projectile.position + Projectile.velocity, Projectile.ai[2] == 5f ? DustID.AncientLight : DustID.GemSapphire, Projectile.velocity);
            d.noGravity = true;

            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.expertMode && Projectile.ai[2] == 5f) target.AddBuff(BuffID.Silenced, 180);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, Projectile.ai[2] == 5f ? DustID.GemDiamond : DustID.GemSapphire, 0f, 0f, 100, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity -= Projectile.oldVelocity * 0.3f;

                int dust2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f, 100, Projectile.ai[2] == 5f ? Color.GhostWhite : Color.Blue, 1f);
                Dust dust3 = Main.dust[dust2];
                dust3.noGravity = true;
                dust3.velocity *= 1.2f;
                dust3.velocity -= Projectile.oldVelocity * 0.3f;
            }
        }
    }
}