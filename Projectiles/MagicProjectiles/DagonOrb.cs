using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Items.Weapons.Magic;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class DagonOrb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        public ref float Timer => ref Projectile.ai[0];
        public bool HitEnemy = false;
        public int MaxBounces = 8;
        public static Color WeakColor => DagonFour.WeakColor;
        public static Color StrongColor => DagonFour.StrongColor;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 14;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 600;
            Projectile.penetrate = MaxBounces;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            if (!HitEnemy)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 25;
                    if (Projectile.alpha < 0) Projectile.alpha = 0;
                }
            }
            else
            {
                Projectile.alpha += 25;
                if (Projectile.alpha >= 255) Projectile.Kill();
            }

            float maxSpeed = 6;
            float currentSpeed = Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y;
            if (currentSpeed < maxSpeed * maxSpeed) Projectile.velocity *= 2.2f;
            if (Projectile.timeLeft < 590) MogModUtils.HomeInOnNPC(Projectile, false, 500f, 13f, 10f);

            if (Projectile.penetrate <= Main.rand.Next(0, MaxBounces + 1))
            {
                Vector2 dustVelocity = new(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                dustVelocity.Normalize();
                dustVelocity *= 50;

                int dagonDust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, Main.rand.NextBool(5) ? DustID.FireworksRGB : DustID.Flare, 0, 0, 100, default, 2.5f);
                Dust dust = Main.dust[dagonDust];
                dust.noGravity = true;
                dust.position.X = Projectile.Center.X;
                dust.position.Y = Projectile.Center.Y;
                dust.position.X += (float)Main.rand.Next(-100, 101) * Timer;
                dust.position.Y += (float)Main.rand.Next(-100, 101) * Timer;

                float dustRot = Main.GlobalTimeWrappedHourly * -5.75f + MathHelper.TwoPi;
                Vector2 dustPos = Projectile.Center + Vector2.UnitX.RotatedBy(dustRot) * 25f * Timer;
                Vector2 dustVel = Vector2.Normalize(dustPos - Projectile.Center).RotatedBy(MathHelper.ToRadians(70)) * 2f * (Timer * 1.5f);
                Dust d = Dust.NewDustPerfect(dustPos, Main.rand.NextBool(2) ? DustID.FireworksRGB : DustID.Flare, dustVel, 100, Color.Lerp(WeakColor, StrongColor, Timer));
                d.noGravity = true;
                d.velocity *= 1.4f;
            }
        }
        public override bool? CanDamage() => !HitEnemy && Projectile.timeLeft < 590;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BlazingDebuff>(), 360);
            KillEffect();
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<BlazingDebuff>(), 360);
            KillEffect();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                KillEffect();
                return true;
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }
        public void KillEffect()
        {
            if (!HitEnemy)
            {
                SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
                for (int i = 0; i < 7; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool(2) ? DustID.FireworksRGB : DustID.Flare, 0f, 0f, 100, WeakColor, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity -= Projectile.oldVelocity * 0.3f;

                    int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Flare, 0f, 0f, 100, StrongColor, 1f);
                    Dust dust3 = Main.dust[dust2];
                    dust3.noGravity = true;
                    dust3.velocity *= 1.2f;
                    dust3.velocity -= Projectile.oldVelocity * 0.3f;
                }
                HitEnemy = true;
                if (Projectile.owner == Main.myPlayer)
                {
                    SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<WeakDagonExplosion>(), (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.SetBlendState(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>("MogMod/Assets/Textures/SmallGrayCircle").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture, drawPosition, null, Color.Lerp(WeakColor, Color.Transparent, Projectile.alpha / 255), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Assets/Textures/GlowParticle").Value;
            Main.EntitySpriteDraw(bloomTex, drawPosition, null, Color.Lerp(StrongColor, Color.Transparent, Projectile.alpha / 255) * 0.5f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.4f, SpriteEffects.None);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float completionRatio = i / (float)Projectile.oldPos.Length;
                Vector2 trailPos = Projectile.oldPos[i] + texture.Size() * 0.5f - Main.screenPosition;

                // The further the smaller
                Color trailColor = Color.Lerp(Color.Lerp(WeakColor, Color.Black, completionRatio), Color.Transparent, Projectile.alpha / 255);
                float trailScale = MathHelper.Lerp(0.7f, 1f, 1f - completionRatio);

                Main.EntitySpriteDraw(texture, trailPos, null, trailColor, 0f, texture.Size() * 0.5f, Projectile.scale * trailScale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}