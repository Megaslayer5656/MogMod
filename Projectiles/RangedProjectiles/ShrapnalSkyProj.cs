using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class ShrapnalSkyProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        private bool initialized = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 150;
            Projectile.light = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 20;

        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (!initialized)
            {
                initialized = true;
                float dustAmt = 16f;
                int d = 0;
                while ((float)d < dustAmt)
                {
                    Vector2 offset = Vector2.UnitX * 0f;
                    offset += -Vector2.UnitY.RotatedBy((double)((float)d * (MathHelper.TwoPi / dustAmt)), default) * new Vector2(1f, 4f);
                    offset = offset.RotatedBy((double)Projectile.velocity.ToRotation(), default);
                    int i = Dust.NewDust(Projectile.Center, 0, 0, DustID.DesertTorch, 0f, 0f, 0, default, 1f);
                    Main.dust[i].scale = 1.5f;
                    Main.dust[i].noGravity = true;
                    Main.dust[i].position = Projectile.Center + offset;
                    Main.dust[i].velocity = Projectile.velocity * 0f + offset.SafeNormalize(Vector2.UnitY) * 1f;
                    d++;
                }
            }
            if (Main.rand.NextBool(25))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Copper, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.9f);
                Main.dust[d].position = Projectile.Center;
                Main.dust[d].noLight = true;
            }
            if (Projectile.timeLeft < 120)
            {
                Projectile.tileCollide = true;
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
        }
    }
}