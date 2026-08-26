using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Classes;
using MogMod.Common.Graphics;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class ShardSpiralProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.ArmorPenetration = 10;
        }
        public override void AI()
        {
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

            //Vector2 value7 = new (5f, 10f);
            //Projectile.ai[2] += 1f;

            //for (int dust = 0; dust < 2; dust++)
            //{
            //    Vector2 dustPosOffset = Vector2.UnitX * -12f;
            //    dustPosOffset = -Vector2.UnitY.RotatedBy((double)(Projectile.ai[2] * 0.1308997f + (float)dust * 3.14159274f), default) * value7 - Projectile.rotation.ToRotationVector2() * 10f;
            //    int exo = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Terragrim, Projectile.velocity.X * 1.5f, Projectile.velocity.Y * 1.5f);
            //    Main.dust[exo].scale = 1.25f;
            //    Main.dust[exo].noGravity = true;
            //    Main.dust[exo].position = Projectile.Center + dustPosOffset;
            //    Main.dust[exo].velocity = Projectile.velocity;
            //}

            Vector2 speed = Projectile.velocity.SafeNormalize(Vector2.Zero);
            if (Main.rand.NextBool(13))
            {
                int num707 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.FireworksRGB, speed.X, speed.Y, 100, Color.PaleTurquoise, 1.2f);
                Main.dust[num707].noGravity = true;
                Dust dust2 = Main.dust[num707];
                dust2.scale *= 1.25f;
                dust2 = Main.dust[num707];
                dust2.velocity *= 2.2f;
                dust2 = Main.dust[num707];
                dust2.velocity += speed * 1.5f;
                dust2 = Main.dust[num707];
                dust2.velocity *= Main.rand.NextFloat();
            }

            MogModUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 500f, 4f, 50f);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Terragrim, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 595)
                return false;
            // draw trail
            TrailDrawer trailDrawer = default;
            Color outerColor = new(0, 105, 128);
            Color innerColor = Color.Turquoise;
            outerColor.A /= 2;
            innerColor.A /= 2;
            trailDrawer.Draw(Projectile, "RainbowRod", outerColor, innerColor, 3f, maxLength: 40f);

            // draw main proj
            Texture2D texture = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/SmallStarParticle").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/CircleGradient").Value;
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