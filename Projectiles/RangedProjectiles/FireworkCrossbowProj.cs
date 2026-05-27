using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class FireworkCrossbowProj : ModProjectile
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 14;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ArmorPenetration = 50;
            Projectile.aiStyle = ProjAIStyleID.FireWork;
            Projectile.tileCollide = true;
            
        }

        public override void AI() //The dust position needs to be tweaked slightly
        {
            int width = Convert.ToInt32(Projectile.width / 2);
            int height = Convert.ToInt32(Projectile.height / 2);
            Vector2 spawn = Projectile.Center - Projectile.velocity / 2f;

            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustPerfect(spawn, DustID.Smoke);
                d.scale = Main.rand.NextFloat(0.8f, 1f);
                d.noGravity = true;
                d.velocity *= 0.1f;
            }
            Dust di = Dust.NewDustPerfect(spawn, DustID.Torch);
            di.scale = Main.rand.NextFloat(1f, 1.2f);
            di.noGravity = true;
            di.velocity *= 0.1f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i ++)
            {
                float SpeedX = (float)Main.rand.NextFloat(-20, 20);
                float SpeedY = (float)Main.rand.NextFloat(-20, 20);
                int d = Dust.NewDust(Projectile.Center, 10, 10, DustID.Firework_Blue, SpeedX, SpeedY, 0, default, 1);
            }
        }
    }
}
