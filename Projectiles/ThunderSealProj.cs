using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace MogMod.Projectiles
{
    public class ThunderSealProj : ModProjectile
    {
        public new string LocalizationCategory => "Projectiles";
        public override string Texture => "MogMod/Projectiles/InvisibleProj";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.extraUpdates = 100;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.alpha = 0;

        }
        public override void AI()
        {
            //float velXMult = 0.85f;
            //Projectile.velocity.X *= velXMult;
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            Vector2 projPos = Projectile.position;
            projPos -= Projectile.velocity;
            int Bolt = Dust.NewDust(projPos, 1, 1, DustID.IchorTorch, 0f, 0f, 0, default, 0.2f);
            Main.dust[Bolt].position = projPos;
            Main.dust[Bolt].scale = Main.rand.Next(10, 30) * 0.014f;
            Main.dust[Bolt].velocity *= 0.8f;
            Main.dust[Bolt].noLight = false;
        }
    }
}
