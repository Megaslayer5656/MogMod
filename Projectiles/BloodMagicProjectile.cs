using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using System.Net.Mail;

namespace MogMod.Projectiles
{
    public class BloodMagicProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.scale = .5f;
            AIType = ProjectileID.Bullet;
        }

        public override void OnKill(int timeLeft)
        {
            //TODO: Add effect when projectile dies
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            int healAmount = (int)(player.statLifeMax2 * 0.0125f);

            player.statLife += healAmount;
            if (player.statLife > player.statLifeMax2)
                player.statLife = player.statLifeMax2;

            player.HealEffect(healAmount);
        }
    }
}
