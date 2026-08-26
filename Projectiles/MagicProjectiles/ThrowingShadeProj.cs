using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class ThrowingShadeProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        private bool hitEnemy = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 420;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            if (Projectile.alpha > 0 && !hitEnemy)
            {
                Projectile.alpha -= 25;
                if (Projectile.alpha < 0) Projectile.alpha = 0;
            }
            if (hitEnemy)
            {
                Projectile.alpha += 35;
                if (Projectile.alpha >= 255) Projectile.Kill();
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.45f / 255f, (255 - Projectile.alpha) * 0.2f / 255f, (255 - Projectile.alpha) * 0.1f / 255f);
            for (int i = 0; i < 2; i++)
            {
                float shortXVel = Projectile.velocity.X / 3f * (float)i;
                float shortYVel = Projectile.velocity.Y / 3f * (float)i;
                int fireDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ShadowbeamStaff, 0f, 0f, 100, default, 1.2f);
                Dust dust = Main.dust[fireDust];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += Projectile.velocity * 0.1f;
                dust.position.X -= shortXVel;
                dust.position.Y -= shortYVel;
            }
            if (Main.rand.NextBool(10))
            {
                int fireDustSmol = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch, 0f, 0f, 100, default, 0.6f);
                Main.dust[fireDustSmol].velocity *= 0.25f;
                Main.dust[fireDustSmol].velocity += Projectile.velocity * 0.5f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitEnemy = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            hitEnemy = true;
            Projectile.velocity = oldVelocity * 0.95f;
            Projectile.position -= Projectile.velocity;
            return false;
        }
        public override bool? CanDamage() => !hitEnemy;
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.DemonTorch, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawer trailDrawer = default;
            Color innerColor = new(76, 0, 143);
            Color outerColor = new(235, 214, 255);
            Color innerDrawColor = Projectile.GetAlpha(innerColor);
            Color outerDrawColor = Projectile.GetAlpha(outerColor);
            trailDrawer.Draw(Projectile, "MogMod:FlameLashRGB", outerDrawColor, innerDrawColor, 1.25f);

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Projectile.GetAlpha(lightColor);
            float rotation = MathHelper.PiOver2 - Main.GlobalTimeWrappedHourly * 2f;
            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, -(rotation * Projectile.direction), texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            return true;
        }
    }
}