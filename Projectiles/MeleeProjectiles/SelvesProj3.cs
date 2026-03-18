using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using Mono.Cecil;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class SelvesProj3 : ModProjectile
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public bool canHit = true;
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.scale = 1f;
            Projectile.netUpdate = true;
            Projectile.alpha = 200;
            Projectile.aiStyle = ProjAIStyleID.Arrow;

            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            Projectile.netUpdate = true;
            Projectile.rotation += MathHelper.ToRadians(-45f);
            Projectile.alpha = 200;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft = 30;
            canHit = false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return canHit;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 1, 1, DustID.ShimmerSpark, 0, 0, 0, default, 1f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 1, 1, DustID.ShimmerSpark, 0, 0, 0, default, 1f);
            }
        }
    }
}
