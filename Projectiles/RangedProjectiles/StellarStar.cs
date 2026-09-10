using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Graphics;
using MogMod.Items.Weapons.Ranged;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class StellarStar : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        private bool hitEnemy = false;
        public int Lifetime = 600;
        public static readonly Color[] colorList =
        [
            StellarBlaster.MainColor1,
            StellarBlaster.MainColor2,
            StellarBlaster.MainColor3
        ];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 54;
            Projectile.alpha = 255;
            Projectile.timeLeft = Lifetime;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Ranged;
        }
        public override void AI()
        {
            if (Projectile.alpha > 0 && !hitEnemy)
            {
                Projectile.alpha -= 15;
                if (Projectile.alpha < 0) Projectile.alpha = 0;
            }
            if (hitEnemy)
            {
                Projectile.alpha += 25;
                if (Projectile.alpha >= 255) Projectile.Kill();
            }

            int helixType = (int)Projectile.ai[1];
            float ep = 0.02f;
            float stein = 6f;
            float krik = (float)helixType * (float)Math.PI;
            float rick = (float)Math.Sin(Projectile.localAI[0] * ((float)Math.PI * 2f) * ep + krik);
            float trick = (float)Math.Sin((Projectile.localAI[0] + 1f) * ((float)Math.PI * 2f) * ep + krik);
            Projectile.localAI[0]++;
            float kirk = trick - rick;
            Vector2 vector = (Projectile.velocity.ToRotation() + (float)Math.PI / 2f).ToRotationVector2();
            Projectile.position += vector * kirk * stein;
            Projectile.rotation = Projectile.velocity.ToRotation();

            Vector2 speed = Projectile.velocity.SafeNormalize(Vector2.Zero);
            float drawSpeed = MathF.Sin(Main.GlobalTimeWrappedHourly * 4) * 0.5f + 0.5f;
            if (Main.rand.NextBool(13))
            {
                int num707 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.FireworksRGB, speed.X, speed.Y, 100, Color.WhiteSmoke, 1.2f);
                Main.dust[num707].noGravity = true;
                Dust dust2 = Main.dust[num707];
                dust2.color = MogModUtils.MulticolorLerp(drawSpeed, colorList);
                dust2.scale *= 1.25f;
                dust2 = Main.dust[num707];
                dust2.velocity *= 2.2f;
                dust2 = Main.dust[num707];
                dust2.velocity += speed * 1.5f;
                dust2 = Main.dust[num707];
                dust2.velocity *= Main.rand.NextFloat();
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity * 0.97f;
            Projectile.position -= Projectile.velocity;
            if (!hitEnemy) SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            hitEnemy = true;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitEnemy = true;
            float drawSpeed = MathF.Sin(Main.GlobalTimeWrappedHourly * 4) * 0.5f + 0.5f;
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int k = 0; k < 3; k++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FireworksRGB, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
                Dust d = Main.dust[dust];
                d.color = MogModUtils.MulticolorLerp(drawSpeed, colorList);
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            hitEnemy = true;
            float drawSpeed = MathF.Sin(Main.GlobalTimeWrappedHourly * 4) * 0.5f + 0.5f;
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int k = 0; k < 3; k++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FireworksRGB, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
                Dust d = Main.dust[dust];
                d.color = MogModUtils.MulticolorLerp(drawSpeed, colorList);
            }
        }
        public override bool? CanDamage() => !hitEnemy;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > Lifetime - 5) return false;
            // draw trail
            TrailDrawer trailDrawer = default;
            float drawSpeed = MathF.Sin(Main.GlobalTimeWrappedHourly * 4) * 0.5f + 0.5f;
            Color outerColor = Projectile.GetAlpha(MogModUtils.MulticolorLerp(drawSpeed, colorList) * 0.1f);
            Color innerColor = Projectile.GetAlpha(MogModUtils.MulticolorLerp(drawSpeed, colorList));
            trailDrawer.Draw(Projectile, "RainbowRod", outerColor, innerColor, 3f, maxLength: 40f);

            // draw main proj
            Texture2D texture = ModContent.Request<Texture2D>("MogMod/Assets/Textures/StarParticle").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Assets/Textures/GlowParticle").Value;
            Vector2 position = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            float rotation = Projectile.rotation;
            Vector2 scale = Vector2.One * Projectile.scale;
            Vector2 newScale = scale + scale * (float)Math.Cos(Main.GlobalTimeWrappedHourly * ((float)Math.PI * 2f)) * 0.2f;
            float x12 = 2f * newScale.X;
            newScale *= 1.0f;
            Vector2 offset = new Vector2(x12, 0f).RotatedBy(rotation);
            // pulsing effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                Main.EntitySpriteDraw(texture, position + offset.RotatedBy(i * ((float)Math.PI * 2f)) * 0f, null, innerColor * (0.5f + i), rotation, texture.Size() * 0.5f, newScale, SpriteEffects.None);
                if (i % 0.5f == 0f) Main.EntitySpriteDraw(bloomTex, position + offset.RotatedBy(i * ((float)Math.PI * 2f)) * 0f, null, outerColor * (0.25f + i), rotation, bloomTex.Size() * 0.5f, newScale * 0.3f, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(texture, position, null, outerColor * 0.5f, rotation, texture.Size() * 0.5f, scale, SpriteEffects.None);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}