using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.Audio;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class BerserkersFireSpearProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.scale = .5f;
            AIType = ProjectileID.JavelinFriendly;
            Projectile.penetrate = 1;
            Projectile.scale = .75f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 300);
            target.AddBuff(BuffID.ShadowFlame, 300);
            target.AddBuff(BuffID.CursedInferno, 300);
            Player player = Main.player[Projectile.owner];
            int healAmount = Convert.ToInt32(player.statLifeMax2 * .0175);
            player.statLife += healAmount;
            if (player.statLife > player.statLifeMax2)
                player.statLife = player.statLifeMax2;
            player.HealEffect(healAmount);
        }

        public override void AI()
        {
            if (Main.rand.NextBool(3))
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
