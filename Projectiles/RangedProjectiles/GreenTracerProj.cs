using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class GreenTracerProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public Color Colour = new(124, 255, 110);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 2;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;

            AIType = ProjectileID.Bullet;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, Colour.ToVector3() * 0.5f);
            Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, Projectile.velocity, 100, default, 0.5f);
            dust.noGravity = true;
            dust.noLight = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GreenTracerDebuff>(), 600);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<GreenTracerDebuff>(), 600);
        }
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 4; i++)
            {
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Terra, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, .5f);         
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

                if (Projectile.timeLeft <= 1550)
                {
                    // backtrail
                    Vector2 trailOffset = Projectile.oldVelocity * 5f;
                    for (float n = 0; n < 4; n++)
                    {
                        Color newColor = Colour * 0.4f;
                        Main.EntitySpriteDraw(bloomTex, drawPosition - (trailOffset * n * 0.05f), null, newColor with { A = 255 }, Projectile.oldRot[(int)(n * 0.05f)], bloomTex.Size() * 0.5f, Projectile.scale * 0.4f, SpriteEffects.None);
                        Main.EntitySpriteDraw(bloomTex, drawPosition - (trailOffset * n * 0.1f), null, newColor with { A = 255 }, Projectile.oldRot[(int)(n * 0.1f)], bloomTex.Size() * 0.5f, Projectile.scale * 0.25f, SpriteEffects.None);
                    }
                }
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}