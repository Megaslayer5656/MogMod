using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles
{
    public class YashaProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles";
        //public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
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
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.alpha = 0;
        }

        public override void AI()
        {
            Vector2 projPos = Projectile.position;
            projPos -= Projectile.velocity;
            
            int yaaasha = Dust.NewDust(projPos, 1, 1, DustID.ShimmerSpark, 0f, 0f, 0, default, 0.5f);
            Main.dust[yaaasha].position = projPos;
            Main.dust[yaaasha].scale = Main.rand.Next(70, 110) * 0.014f;
            Main.dust[yaaasha].velocity *= 0.2f;
        }
    }
}