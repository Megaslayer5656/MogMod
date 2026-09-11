using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Graphics;
using MogMod.Items.Weapons.Ranged;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
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
        public ref float Scale => ref Projectile.ai[2];
        private bool hitEnemy = false;
        public int Lifetime = 600;
        public int MaxHits = 3;
        public int MaxPenetrate = 1;
        public int Size = 0;
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
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.hide = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override void AI()
        {
            // something in here is causing the consequent to be less than 1
            // it causes the proj to instantly disappear
            if (hitEnemy)
            {
                //Projectile.localNPCHitCooldown = -1;
                SoundEngine.PlaySound(SoundID.Item117 with { Volume = 0.8f, Pitch = -0.1f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot with { Pitch = 0.15f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Pitch = 0.2f }, Projectile.Center);
                if (StarCharge >= 1f)
                {
                    Projectile.position = Projectile.Center;
                    Projectile.width *= 2;
                    Projectile.height *= 2;
                    Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                    if (Projectile.owner == Main.myPlayer) Projectile.Damage();
                }

                Projectile.alpha += 5;
                if (Projectile.alpha >= 255) Projectile.Kill();
            }

            MaxPenetrate = (int)(MaxHits * (StarCharge + 2f));
            Projectile.Center = Projectile.position;
            Projectile.position = Projectile.Center;

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
            if (Main.rand.NextBool(13 - Projectile.numHits))
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

            Scale += 0.6f * (hitEnemy ? 3f : 1f);
            float scaleMax = Projectile.scale * 3f * (hitEnemy ? 3f : 1f);
            Scale = MathHelper.Clamp(Scale, 0f, scaleMax);
            Lighting.AddLight(Projectile.Center, (MogModUtils.MulticolorLerp(drawSpeed, colorList).ToVector3() * 0.01f) * Scale);

            MogModUtils.HomeInOnNPC(Projectile, true, 1200f, 12f, 30f, false);
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            float mult = 1.5f * Scale;
            Size = (int)Utils.Remap(StarCharge, 0f, 120 * mult, 10f, 125f * mult);
            hitbox.Inflate(Size, Size);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits >= MaxPenetrate) hitEnemy = true;
            float drawSpeed = MathF.Sin(Main.GlobalTimeWrappedHourly * 4) * 0.5f + 0.5f;
            for (int k = 0; k < 3; k++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FireworksRGB, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
                Dust d = Main.dust[dust];
                d.color = MogModUtils.MulticolorLerp(drawSpeed, colorList);
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.numHits >= MaxPenetrate) hitEnemy = true;
            float drawSpeed = MathF.Sin(Main.GlobalTimeWrappedHourly * 4) * 0.5f + 0.5f;
            for (int k = 0; k < 3; k++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FireworksRGB, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
                Dust d = Main.dust[dust];
                d.color = MogModUtils.MulticolorLerp(drawSpeed, colorList);
            }
        }
        public override bool? CanDamage() => Projectile.timeLeft <= Lifetime - 2;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);
        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawer trailDrawer = default;
            Color auraColor = Color.White;
            float scale = Math.Abs(1f - (Scale * 0.075f));
            if (Projectile.timeLeft <= Lifetime - 2) trailDrawer.Draw(Projectile, "RainbowRod", auraColor * 0.1f, auraColor, scale, maxLength: 40f);

            var starTex = ModContent.Request<Texture2D>("MogMod/Assets/Textures/StarParticle").Value;
            var bloomTex = ModContent.Request<Texture2D>("MogMod/Assets/Textures/GlowParticle").Value;
            Vector2 starPos = Projectile.Center - Main.screenPosition;
            float opacity = StarCharge * 2.5f;
            float newDrawTimer = StarCharge * 5f;
            auraColor = (StarCharge >= 1f ? MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * newDrawTimer, colorList) : StarCharge >= 0.5f ? MogModUtils.MulticolorLerp(newDrawTimer, colorList) : Color.WhiteSmoke) * opacity * 0.8f;
            float newScale = Scale + Scale * (float)Math.Cos(Main.GlobalTimeWrappedHourly * ((float)Math.PI * 2f)) * 0.2f;

            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (float i = MathHelper.PiOver2; i <= MathHelper.Pi; i += MathHelper.PiOver4)
            {
                float starRotation = (newDrawTimer * ((Projectile.timeLeft <= Lifetime - 2 || StarCharge >= 1f) ? Main.GlobalTimeWrappedHourly : 1.75f) * Scale) + i;
                Color Transparency = Projectile.GetAlpha(auraColor) * (opacity * (StarCharge / i));
                Main.EntitySpriteDraw(bloomTex, starPos, null, Transparency * 0.5f, starRotation, bloomTex.Size() * 0.5f, newScale * 0.2f * (newDrawTimer / 3f), SpriteEffects.None, 0);
                Main.EntitySpriteDraw(starTex, starPos, null, Transparency, starRotation, starTex.Size() * 0.5f, Scale * (newDrawTimer / 3f), SpriteEffects.None, 0);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}