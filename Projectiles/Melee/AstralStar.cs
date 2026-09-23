using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Graphics;
using MogMod.Items.Weapons.Melee;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    // lingers in the air (no gravity)
    // can be hit to set proj ai[2] to 5 and launch it to cursor
    public class AstralStar : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        private bool hitEnemy = false;
        public const int Lifetime = 480;
        public int MaxHits = 10;
        public float MinVel = 2f;
        public static readonly Color[] colorList =
        [
            AstralCataclysm.MainColor1,
            AstralCataclysm.MainColor2,
            AstralCataclysm.MainColor3
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
            Projectile.timeLeft = Lifetime + 40;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.netImportant = true;
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

            int helixType = (int)Projectile.ai[2];
            float ep = Projectile.ai[1] == 1f ? 0.08f : 0.02f;
            float stein = Projectile.ai[1] == 1f ? 12f : 3f;
            float krik = (float)helixType * (float)Math.PI;
            float rick = (float)Math.Sin(Projectile.localAI[0] * ((float)Math.PI * 2f) * ep + krik);
            float trick = (float)Math.Sin((Projectile.localAI[0] + 1f) * ((float)Math.PI * 2f) * ep + krik);
            Projectile.localAI[0]++;
            float kirk = trick - rick;
            Vector2 vector = (Projectile.velocity.ToRotation() + (float)Math.PI / 2f).ToRotationVector2();
            Projectile.position += vector * kirk * stein;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.ai[1] == 1f)
            {
                Projectile.netUpdate = true;
                MogModUtils.HomeInOnNPC(Projectile, true, 1200f, 12f, 30f, false);
                if (Projectile.timeLeft < Lifetime - 240) Projectile.ai[1] = 0f;
            }
            else
            {
                Projectile.netUpdate = true;
                if (Projectile.velocity.Length() > 8) Projectile.velocity *= 0.88f;
                else
                {
                    Projectile.velocity *= 0.965f;
                    if (Projectile.ai[1] >= 2f && Projectile.velocity.Length() <= MinVel) Projectile.ai[1] = 0f;
                }
            }
            //Main.NewText($"{Projectile.velocity.Length()}");

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
            Projectile.numHits++;
            if (Projectile.numHits >= MaxHits) hitEnemy = true;
            else
            {
                if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;
            }
            if (!hitEnemy) SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.65f, Pitch = 0.35f, PitchVariance = 0.6f }, Projectile.Center);
            Projectile.netUpdate = true;
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
            Projectile.netUpdate = true;
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
            Projectile.netUpdate = true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[1] >= 1f)
            {
                modifiers.SourceDamage *= 2f;
                modifiers.Knockback += 1f;
            }
        }
        public override bool? CanDamage() => !hitEnemy && ((Projectile.ai[1] == 1f) || (Projectile.ai[1] >= 2f && Projectile.velocity.Length() > MinVel));
        public override bool PreDraw(ref Color lightColor)
        {
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
            Vector2 scale = Vector2.One * Projectile.scale;
            Vector2 newScale = scale + scale * (float)Math.Cos(Main.GlobalTimeWrappedHourly * ((float)Math.PI * 2f)) * 0.2f;
            float x12 = 2f * newScale.X;
            newScale *= 1.0f;
            float starRotation = MathHelper.PiOver2 - Main.GlobalTimeWrappedHourly * 8f;
            Vector2 offset = new Vector2(x12, 0f).RotatedBy(starRotation);
            // pulsing effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (float i = 0f; i < 1f; i += (Projectile.ai[1] >= 1f ? 0.125f: 0.25f))
            {
                Main.EntitySpriteDraw(texture, position + offset.RotatedBy(i * ((float)Math.PI * 2f)) * 0f, null, innerColor * (0.5f + i), starRotation, texture.Size() * 0.5f, newScale * (Projectile.ai[1] >= 1f ? 1.25f : 1f), SpriteEffects.None);
                if (Projectile.ai[1] >= 1f) Main.EntitySpriteDraw(bloomTex, position + offset.RotatedBy(i * ((float)Math.PI * 2f)) * 0f, null, innerColor * (0.5f + i), starRotation, bloomTex.Size() * 0.5f, newScale * 0.15f, SpriteEffects.None);
                else if (i % 0.5f == 0f) Main.EntitySpriteDraw(bloomTex, position + offset.RotatedBy(i * ((float)Math.PI * 2f)) * 0f, null, outerColor * (0.25f + i), starRotation, bloomTex.Size() * 0.5f, newScale * 0.3f, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(texture, position, null, outerColor * 0.5f, starRotation, texture.Size() * 0.5f, scale, SpriteEffects.None);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}