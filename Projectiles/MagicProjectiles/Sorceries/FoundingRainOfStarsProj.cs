using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Classes;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class FoundingRainOfStarsProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/SmallGrayCircle";
        public static Color Colour => new(211, 209, 255);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.timeLeft = 200;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = SorceryDamageClass.Instance;
        }
        public override void AI()
        {
            float maxSpeed = 15;
            float currentSpeed = Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y;

            Projectile.velocity.X *= 0.98f;

            Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Projectile.velocity, 100, Color.LightBlue, 1f);
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.91f, 1.417f);
            dust.velocity *= 0.1f;

            if (Projectile.timeLeft <= 150 && Projectile.timeLeft % 30 == 0)
            {
                Projectile.velocity *= 0f;
                SummonLasers();
            }
            // y velocity stopping after reaching max speed;
            if (Projectile.velocity.Y > 0)
                if (currentSpeed < maxSpeed * maxSpeed)
                    Projectile.velocity.Y *= -1.05f;
            if (Projectile.velocity.Y <= 0)
            {
                if (currentSpeed < (maxSpeed * -1) * (maxSpeed * -1))
                    Projectile.velocity.Y *= 1.05f;
                else
                    Projectile.velocity.Y -= .1f;
            }
            // speeds up y velocity when shot straight forward;
            if (Projectile.velocity.Y <= 5 && Projectile.timeLeft > 150)
            {
                Projectile.velocity.Y -= .25f;
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 7; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f, 100, Color.LightBlue, .8f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity -= Projectile.oldVelocity * 0.3f;

                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire, 0f, 0f, 100, default, .8f);
                Dust dust3 = Main.dust[dust2];
                dust3.noGravity = true;
                dust3.velocity *= 1.2f;
                dust3.velocity -= Projectile.oldVelocity * 0.3f;
            }
        }
        public override bool? CanDamage() => false;
        private void SummonLasers()
        {
            var source = Projectile.GetSource_FromThis();
            Vector2 spawnPos = Projectile.Center + new Vector2(0, 500);
            SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
            for (int n = 0; n < 4; n++)
            {
                MogModUtils.ProjectileRain(source, spawnPos, 250f, 50f, 800f, 800f, 15f, ModContent.ProjectileType<FoundingRainOfStarsStarProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.SetBlendState(BlendState.Additive);

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture, drawPosition, null, Colour, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/CircleGradient").Value;
            Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour * 0.5f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.25f, SpriteEffects.None);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float completionRatio = i / (float)Projectile.oldPos.Length;
                Vector2 trailPos = Projectile.oldPos[i] + texture.Size() * 0.5f - Main.screenPosition;

                // The further the smaller
                Color trailColor = Color.Lerp(Colour, Color.Black, completionRatio);
                float trailScale = MathHelper.Lerp(0.5f, 1f, 1f - completionRatio);

                Main.EntitySpriteDraw(texture, trailPos, null, trailColor, 0f, texture.Size() * 0.5f, Projectile.scale * trailScale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}