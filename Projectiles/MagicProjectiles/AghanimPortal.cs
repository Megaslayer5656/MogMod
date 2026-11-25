using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class AghanimPortal : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
        }
        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source) => SoundEngine.PlaySound(SoundID.Item117, Projectile.Center);
        public override void AI()
        {
            Projectile.rotation += 0.05f;
            // drift to a stop after being launched
            if (Projectile.timeLeft < 580)
                Projectile.velocity *= 0.882f;
            var source = Projectile.GetSource_FromThis();
            if ((Projectile.timeLeft == 550))
                Projectile.NewProjectile(source, Projectile.Center, Projectile.velocity, ModContent.ProjectileType<AghanimLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            if (Main.rand.NextBool(3))
                for (int i = 0; i < 4; i++)
                {
                    int purpleDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmethyst);
                    Main.dust[purpleDust].noGravity = true;
                    Main.dust[purpleDust].scale = 1.75f;
                }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 70)
            {
                float timeAlpha = (float)Projectile.timeLeft / 10f;
                Projectile.alpha = (int)(255f - 255f * timeAlpha);
            }
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(texture, drawPosition, null, Color.BlueViolet, Projectile.rotation * 1.2f, texture.Size() * 0.5f, Projectile.scale * 1.55f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, drawPosition, null, Projectile.GetAlpha(lightColor * 0.8f), Projectile.rotation * 0.8f, texture.Size() * 0.5f, Projectile.scale * 1.05f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, drawPosition, null, Projectile.GetAlpha(lightColor * 0.8f), -Projectile.rotation * 0.8f, texture.Size() * 0.5f, Projectile.scale * 1.05f, SpriteEffects.FlipHorizontally, 0);

            // draw glow effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Assets/Textures/GlowParticle").Value;
            for (int i = 0; i < 4; i++)
            {
                float increment = MathHelper.Lerp(1f, 1.5f, MathF.Sin(Main.GlobalTimeWrappedHourly * 2) * 0.5f + 0.5f);
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, Color.Orchid * 0.85f, 0f, bloomTex.Size() * 0.5f, Projectile.scale * 0.7f * increment, SpriteEffects.None);
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, Color.MediumPurple * 0.1f, 0f, bloomTex.Size() * 0.5f, Projectile.scale * 0.8f * increment, SpriteEffects.None);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}