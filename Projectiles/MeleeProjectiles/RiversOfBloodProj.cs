using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class RiversOfBloodProj : ModProjectile
    {
        public override void SetDefaults() //TODO: Fix this projectile's janky hitbox
        {
            Projectile.width = 76;
            Projectile.height = 30;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.scale = 2f;

            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.position, 1f, 0f, 0f);
            if (Main.rand.NextBool(5))
            {
                int blood = Dust.NewDust(Projectile.position, 10, 60, DustID.Blood, 0f, 0f, 150, default, 3f);
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 10; i++)
            {
                int blood = Dust.NewDust(target.Center, 8, 8, DustID.Blood, 0, 0, 0, default, 2f);
            }
        }
    }
}
