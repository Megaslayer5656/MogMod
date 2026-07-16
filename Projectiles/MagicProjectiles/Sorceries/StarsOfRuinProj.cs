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
    public class StarsOfRuinProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/SmallGrayCircle";
        public static Color Colour => new(211, 209, 255);
        public override void SetStaticDefaults()
        {
            // required for texture drawing
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        int numb = 0;
        float offset = Main.rand.NextFloat(MathHelper.TwoPi);
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.timeLeft = 80;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = SorceryDamageClass.Instance;
        }
        public override void AI()
        {
            Projectile.velocity *= 0.95f;

            Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Projectile.velocity, 100, Color.LightBlue, 1f);
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.91f, 1.417f);
            dust.velocity *= 0.1f;

            if (Projectile.timeLeft <= 60 && Projectile.timeLeft % 5 == 0)
            {
                numb++;
                SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                Vector2 velocity = ((MathHelper.TwoPi * numb / 6f) - offset).ToRotationVector2() * 3f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<StarsOfRuinHomingProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
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