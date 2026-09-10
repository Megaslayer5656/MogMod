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
    public class ChargedStellarStar : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        public ref float StarCharge => ref Projectile.ai[0];
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
            Projectile.timeLeft = Lifetime;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Ranged;
        }
        public override void AI()
        {
            if (hitEnemy)
            {
                Projectile.alpha += 25;
                if (Projectile.alpha >= 255) Projectile.Kill();
            }

            Main.NewText($"ai2 == {StarCharge}");

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
            float drawSpeed = MathF.Sin(Main.GlobalTimeWrappedHourly * 4 * (StarCharge + 1f)) * 0.5f + 0.5f;
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
            if (StarCharge >= 0.66f) return false;
            Projectile.velocity = oldVelocity * 0.97f;
            Projectile.position -= Projectile.velocity;
            if (!hitEnemy) SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            hitEnemy = true;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //if (Projectile.numHits >= value * StarCharge)
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
            if (Projectile.timeLeft > Lifetime - 2) return false;

            Color auraColor = Color.White;
            var starTex = ModContent.Request<Texture2D>("MogMod/Assets/Textures/StarParticle").Value;
            var bloomTex = ModContent.Request<Texture2D>("MogMod/Assets/Textures/GlowParticle").Value;
            Vector2 starPos = Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.75f - Main.screenPosition;
            float opacity = StarCharge;
            float newDrawTimer = StarCharge * 5f;
            auraColor = (StarCharge >= 0.66f ? MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * newDrawTimer, colorList) : StarCharge >= 0.33f ? MogModUtils.MulticolorLerp(newDrawTimer, colorList) : Color.WhiteSmoke) * opacity * 0.8f;
            float newScale = Projectile.scale + Projectile.scale * (float)Math.Cos(Main.GlobalTimeWrappedHourly * ((float)Math.PI * 2f)) * 0.2f;

            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (float i = MathHelper.PiOver2; i <= MathHelper.Pi; i += MathHelper.PiOver4)
            {
                float starRotation = (Main.GlobalTimeWrappedHourly * newDrawTimer) + i;
                Color Transparency = Projectile.GetAlpha(auraColor) * (opacity * (StarCharge / i));
                Main.EntitySpriteDraw(bloomTex, starPos, null, Transparency, starRotation, bloomTex.Size() * 0.5f, newScale * 0.15f * (newDrawTimer / 3f), SpriteEffects.None, 0);
                Main.EntitySpriteDraw(starTex, starPos, null, Transparency, starRotation, starTex.Size() * 0.5f, Projectile.scale * (newDrawTimer / 3f), SpriteEffects.None, 0);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}