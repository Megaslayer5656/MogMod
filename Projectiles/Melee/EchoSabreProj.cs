using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class EchoSabreProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public Color Colour = new(185, 255, 153);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 2;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 30;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 3f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
            float dustAmt = 16f;
            int d = 0;
            while ((float)d < dustAmt)
            {
                Vector2 offset = Vector2.UnitX * 0f;
                offset += -Vector2.UnitY.RotatedBy((double)((float)d * (MathHelper.TwoPi / dustAmt))) * new Vector2(1f, 4f);
                offset = offset.RotatedBy((double)Projectile.velocity.ToRotation());
                int i = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowTorch, 0f, 0f, 0, Color.LightGreen, 1f);
                Main.dust[i].scale = 1f;
                Main.dust[i].noLight = true;
                Main.dust[i].noGravity = true;
                Main.dust[i].velocity = Projectile.velocity * 0f + offset.SafeNormalize(Vector2.UnitY) * 1f;
                d++;
            }
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, Colour.ToVector3() * 0.5f);
            float pi = MathHelper.Pi;
            for (int d = 0; d < 1; d++)
            {
                int i = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ShimmerSpark, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 160, Color.LightGreen, 2.4f);
                Main.dust[i].scale = 0.5f;
                Main.dust[i].noLight = true;
                Main.dust[i].noGravity = true;
                Main.dust[i].velocity *= .6f;
                Main.dust[i].fadeIn = 0.2f;
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            float dustAmt = 16f;
            int d = 0;
            Projectile.ai[0]++;
            while ((float)d < dustAmt)
            {
                Vector2 offset = Vector2.UnitX * 0f;
                offset += -Vector2.UnitY.RotatedBy((double)((float)d * (MathHelper.TwoPi / dustAmt))) * new Vector2(1f, 4f);
                offset = offset.RotatedBy((double)Projectile.velocity.ToRotation());
                int i = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowTorch, 0f, 0f, 0, Color.LightGreen, 1f);
                Main.dust[i].scale = 1f;
                Main.dust[i].noLight = true;
                Main.dust[i].noGravity = true;
                Main.dust[i].velocity = Projectile.velocity * 0f + offset.SafeNormalize(Vector2.UnitY) * 1f;
                d++;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // draw glow effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/StarProj").Value;
            for (int i = 0; i < 2; i++)
            {
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None);

                if (Projectile.ai[0] >= 5f)
                {
                    // backtrail
                    Vector2 trailOffset = Projectile.oldVelocity * 5f;
                    for (float n = 0; n < 4; n++)
                    {
                        Color newColor = Colour * 0.4f;
                        Main.EntitySpriteDraw(bloomTex, drawPosition - (trailOffset * n * 0.05f), null, newColor with { A = 255 }, Projectile.oldRot[(int)(n * 0.05f)], bloomTex.Size() * 0.5f, Projectile.scale * 0.8f, SpriteEffects.None);
                        Main.EntitySpriteDraw(bloomTex, drawPosition - (trailOffset * n * 0.1f), null, newColor with { A = 255 }, Projectile.oldRot[(int)(n * 0.1f)], bloomTex.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None);
                    }
                }
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}