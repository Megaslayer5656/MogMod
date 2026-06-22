using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class VoCProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.timeLeft = 180;
        }
        public override void AI()
        {
            MogModUtils.HomeInOnNPC(Projectile, true, 500f, 14f, 20f);
            for (int i = 0; i < 2; i++)
            {
                float velocityX = Projectile.velocity.X / 3f * (float)i;
                float velocityY = Projectile.velocity.Y / 3f * (float)i;
                int waterFlame = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? DustID.Flare : 217, 0f, 0f, 100, default, 1.2f);
                Dust dust = Main.dust[waterFlame];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += Projectile.velocity * 0.1f;
                dust.position.X -= velocityX;
                dust.position.Y -= velocityY;
            }
            if (Main.rand.NextBool(10))
            {
                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? DustID.Torch : 176, 0f, 0f, 100, default, 0.6f);
                Main.dust[dust2].velocity *= 0.25f;
                Main.dust[dust2].velocity += Projectile.velocity * 0.5f;
            }
            Projectile.rotation += 0.3f * (float)Projectile.direction;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 180);
            target.AddBuff(BuffID.Wet, 180);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            for (int k = 0; k < 15; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? DustID.Flare : 217, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            for (int i = 0; i < 9; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? DustID.Torch : 176, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, 100, default, 1.7f);
                Main.dust[dust].velocity *= 1.4f;
            }
        }
    }
}