using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class MoonveilProj : ModProjectile
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 40;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.scale = 1.25f;
            Projectile.netUpdate = true;

            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.position, 1f, 1f, 1f);
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(Projectile.position, 10, 60, DustID.IceTorch, 0f, 0f, 150, default, 3f);
            }

            Projectile.netUpdate = true;
        }
    }
}
