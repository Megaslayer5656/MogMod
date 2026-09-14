using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Classes;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Classless
{
    // TODO: fix proj explosion not syncing in multiplayer
    public class DragonPhantomProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Classless";
        public ref float Scale => ref Projectile.ai[0];
        private bool PlayedSounds = false;
        private bool HitEnemy = false;
        public float FadeOut = 1.5f;
        public int Size = 0;
        public int Lifetime = 600;
        public int NumAnimationFrames = 8;
        public int AnimationFrameTime = 5;
        public Color Color1 = Color.Silver;
        public Color Color2 = Color.Crimson;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            Main.projFrames[Projectile.type] = NumAnimationFrames;
        }
        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 76;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.DamageType = MeleeRangedDamageClass.Instance;
            Projectile.hide = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsySummon, Projectile.Center);
            if (Projectile.ai[1] >= 5f)
            {
                Projectile.localNPCHitCooldown = 2;
                Color1 = Color.Goldenrod;
            }
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % (AnimationFrameTime) == 0) Projectile.frame = Projectile.frame >= NumAnimationFrames - 1 ? 0 : Projectile.frame + 1;

            if (HitEnemy || Projectile.timeLeft <= Lifetime / 10)
            {
                Projectile.alpha += 35;
                Projectile.localNPCHitCooldown = -1;
                if ((HitEnemy || Projectile.timeLeft > Lifetime / 10) && Projectile.ai[1] >= 5f)
                {
                    if (!PlayedSounds)
                    {
                        SoundEngine.PlaySound(SoundID.Item117 with { Volume = 0.8f, Pitch = -0.1f }, Projectile.Center);
                        SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.Center);
                        SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot with { Pitch = 0.15f }, Projectile.Center);
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { Pitch = 0.2f }, Projectile.Center);
                        PlayedSounds = true;
                    }
                    if (Projectile.alpha < 255)
                    {
                        Projectile.velocity = Vector2.Zero;
                        Projectile.position = Projectile.Center;
                        Projectile.width = (int)(Projectile.width * 1.3f);
                        Projectile.height = (int)(Projectile.height * 1.3f);
                        Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                        Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                        if (Projectile.owner == Main.myPlayer) Projectile.Damage();
                    }
                    if (Projectile.alpha >= 125) FadeOut -= 0.05f;
                }
                if ((Projectile.alpha >= 255 && FadeOut >= 1f) || FadeOut <= 0f) Projectile.Kill();
            }

            float drawSpeed = MathF.Sin(Main.GlobalTimeWrappedHourly * 4 * (Scale + 1f)) * 0.5f + 0.5f;
            if (!HitEnemy)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.spriteDirection = Projectile.direction;
                if (Main.rand.NextBool(3)) Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FireworksRGB, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 100, Color.Lerp(Color1, Color2, drawSpeed));
            }


            float mult = (HitEnemy ? 1.5f : 1f);
            Scale += 0.6f * mult;
            float scaleMax = Projectile.scale * 3f * mult;
            Scale = MathHelper.Clamp(Scale, 0f, scaleMax);
            Lighting.AddLight(Projectile.Center, (Color.Lerp(Color1, Color2, drawSpeed).ToVector3() * 0.01f) * Scale);

            if (Projectile.alpha <= 25) MogModUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 850f, 10f, 25f);
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            float mult = 1.5f * Scale;
            Size = (int)Utils.Remap(Scale, 0f, 120 * mult, 10f, 125f * mult);
            hitbox.Inflate(Size, Size);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HitEnemy = true;
            float drawSpeed = MathF.Sin(Main.GlobalTimeWrappedHourly * 4) * 0.5f + 0.5f;
            for (int k = 0; k < 3; k++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FireworksRGB, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
                Dust d = Main.dust[dust];
                d.color = Color.Lerp(Color1, Color2, drawSpeed);
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            HitEnemy = true;
            float drawSpeed = MathF.Sin(Main.GlobalTimeWrappedHourly * 4) * 0.5f + 0.5f;
            for (int k = 0; k < 3; k++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FireworksRGB, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
                Dust d = Main.dust[dust];
                d.color = Color.Lerp(Color1, Color2, drawSpeed);
            }
        }
        public override bool? CanDamage() => (Projectile.timeLeft <= Lifetime - 2 && Projectile.alpha < 255);
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Color auraColor = Color.White;
            float scale = Math.Abs(1f - (Scale * 0.075f));
            float opacity = Scale * 2.5f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Rectangle sourceRectangle = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);
            SpriteEffects direction = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Main.EntitySpriteDraw(tex, drawPos + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Scale * 0.5f, direction, 0);
            
            // explosion visual
            if (!HitEnemy || Projectile.ai[1] < 5f) return false;
            var bloomTex = ModContent.Request<Texture2D>("MogMod/Assets/Textures/GlowParticle").Value;
            float newDrawTimer = Scale * 5f;
            auraColor = Color.Lerp(Color1, Color2, Main.GlobalTimeWrappedHourly * newDrawTimer) * opacity;
            float newScale = Scale + Scale * (float)Math.Cos(Main.GlobalTimeWrappedHourly * ((float)Math.PI * 2f)) * 0.2f;
            float mult = (HitEnemy ? (Projectile.alpha / 30) : 1f);

            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (float i = MathHelper.PiOver2; i <= MathHelper.TwoPi; i += MathHelper.PiOver4)
            {
                float starRotation = (newDrawTimer * ((Projectile.timeLeft <= Lifetime - 2) ? Main.GlobalTimeWrappedHourly : 1.75f) * Scale) + i;
                Color trans = Projectile.GetAlpha(auraColor) * MathHelper.Min(FadeOut, 1f);
                Main.EntitySpriteDraw(bloomTex, drawPos, null, trans, starRotation, bloomTex.Size() * 0.5f, newScale * 0.2f * mult, SpriteEffects.None, 0);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}