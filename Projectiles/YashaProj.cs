using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace MogMod.Projectiles
{
    public class YashaProj : ModProjectile, ILocalizedModType
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
            int yaaasha = Dust.NewDust(projPos, 1, 1, DustID.ShimmerSpark, 0f, 0f, 0, default, 0.2f);
            Main.dust[yaaasha].position = projPos;
            Main.dust[yaaasha].scale = Main.rand.Next(10, 30) * 0.014f;
            Main.dust[yaaasha].velocity *= 0.8f;
            Main.dust[yaaasha].noLight = true;
        }
        //public override void OnKill(int timeLeft)
        //{
        //    SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        //    //Dust splash
        //    int dustsplash = 0;
        //    while (dustsplash < 4)
        //    {
        //        int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 0.9f);
        //        Main.dust[d].position = Projectile.Center;
        //        dustsplash += 1;
        //    }
        //}
    }
}