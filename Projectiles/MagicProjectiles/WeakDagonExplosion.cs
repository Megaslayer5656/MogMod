using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Weapons.Magic;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class WeakDagonExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        public ref float Timer => ref Projectile.ai[0];
        private const float radius = 20f;
        public static Color WeakColor => DagonThree.WeakColor;
        public static Color StrongColor => DagonThree.StrongColor;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Magic;
        }
        public override void AI()
        {
            Timer += 0.2f;
            if (Projectile.timeLeft >= 8)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 dustVelocity = new(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    dustVelocity.Normalize();
                    dustVelocity *= 50;

                    int dagonDust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Flare, 0, 0, 100, default, 2.5f);
                    Dust dust = Main.dust[dagonDust];
                    dust.noGravity = true;
                    dust.position.X = Projectile.Center.X;
                    dust.position.Y = Projectile.Center.Y;
                    dust.position.X += (float)Main.rand.Next(-((int)radius * 2), ((int)radius * 2) + 1) * Timer;
                    dust.position.Y += (float)Main.rand.Next(-((int)radius * 2), ((int)radius * 2) + 1) * Timer;
                }
                int dustNum = (int)MathHelper.Clamp(Timer * 5f, 1f, 5f);
                for (int s = 0; s < dustNum; s++)
                {
                    float dustRot = Main.GlobalTimeWrappedHourly * -5.75f + (MathHelper.TwoPi / dustNum * s);
                    Vector2 dustPos = Projectile.Center + Vector2.UnitX.RotatedBy(dustRot) * 25f * Timer;
                    Vector2 dustVel = Vector2.Normalize(dustPos - Projectile.Center).RotatedBy(MathHelper.ToRadians(70)) * 2f * (Timer * 1.5f);
                    Dust d = Dust.NewDustPerfect(dustPos, Main.rand.NextBool(5) ? DustID.FireworksRGB : DustID.Flare, dustVel, 100, Color.Lerp(WeakColor, StrongColor, Timer));
                    d.noGravity = true;
                    d.velocity *= 1.4f;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire3, 240);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.OnFire3, 240);
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => MogModUtils.CircularHitboxCollision(Projectile.Center, radius * MathHelper.Max(1f, Timer), targetHitbox);
        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer > 18) return false;
            // draw glow effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Texture2D ringTex = ModContent.Request<Texture2D>("MogMod/Assets/Textures/VerticalSmearThin").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Assets/Textures/GlowParticle").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColour = Projectile.GetAlpha(Color.Lerp(WeakColor, StrongColor, Timer)) * MathHelper.Min(1f, Timer);
            float rotation = MathHelper.PiOver2 - Main.GlobalTimeWrappedHourly * -(8f * Timer);

            for (int i = 0; i < 8; i++)
            {
                if (i % 2 == 0)
                {
                    Main.EntitySpriteDraw(ringTex, drawPosition, null, drawColour * 0.85f, rotation, ringTex.Size() * 0.5f, Projectile.scale * Timer * 0.05f, SpriteEffects.None);
                    Main.EntitySpriteDraw(ringTex, drawPosition, null, drawColour * 0.85f, rotation + MathHelper.Pi, ringTex.Size() * 0.5f, Projectile.scale * Timer * 0.05f, SpriteEffects.None);
                }
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, drawColour * 0.75f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * ((Timer * 0.0375f) * i), SpriteEffects.None);
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, drawColour * 0.35f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * ((Timer * 0.05f) * i), SpriteEffects.None);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}