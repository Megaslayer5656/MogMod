using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Items.Weapons.Melee;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class FlamewallExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public ref float Timer => ref Projectile.ai[2];
        private const float radius = 30f;
        public static Color WeakColor => Flamewall.WeakColor;
        public static Color StrongColor => Flamewall.StrongColor;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void AI()
        {
            if (Projectile.timeLeft >= 18)
            {
                Timer += 0.1f;
                for (int i = 0; i < 8; i++)
                {
                    Vector2 dustVelocity = new(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    dustVelocity.Normalize();
                    dustVelocity *= 50;

                    int dagonDust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, Main.rand.NextBool(5) ? DustID.FireworksRGB : DustID.Flare, 0, 0, 100, default, 2.5f);
                    Dust dust = Main.dust[dagonDust];
                    dust.noGravity = true;
                    dust.position.X = Projectile.Center.X;
                    dust.position.Y = Projectile.Center.Y;
                    dust.position.X += (float)Main.rand.Next(-((int)radius * 2), ((int)radius * 2) + 1) * Timer;
                    dust.position.Y += (float)Main.rand.Next(-((int)radius * 2), ((int)radius * 2) + 1) * Timer;
                }
                int dustNum = (int)MathHelper.Clamp(Timer * 6f, 1f, 6f);
                for (int s = 0; s < dustNum; s++)
                {
                    float dustRot = Main.GlobalTimeWrappedHourly * -5.75f + (MathHelper.TwoPi / dustNum * s);
                    Vector2 dustPos = Projectile.Center + Vector2.UnitX.RotatedBy(dustRot) * 25f * Timer;
                    Vector2 dustVel = Vector2.Normalize(dustPos - Projectile.Center).RotatedBy(MathHelper.ToRadians(70)) * 2f * (Timer * 1.5f);
                    Dust d = Dust.NewDustPerfect(dustPos, Main.rand.NextBool(2) ? DustID.FireworksRGB : DustID.Flare, dustVel, 100, Color.Lerp(WeakColor, StrongColor, Timer));
                    d.noGravity = true;
                    d.velocity *= 1.4f;
                }
            }
            else Timer -= 0.2f;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.SourceDamage *= (Owner.MogMod().flamewallPower * Flamewall.DamageMult);
            modifiers.Knockback += Owner.MogMod().flamewallPower;
            modifiers.CritDamage *= (Owner.MogMod().flamewallPower * Flamewall.CritMult);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BlazingDebuff>(), 360);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<BlazingDebuff>(), 360);
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => MogModUtils.CircularHitboxCollision(Projectile.Center, radius * MathHelper.Max(1f, Timer), targetHitbox);
        public override bool PreDraw(ref Color lightColor)
        {
            // draw glow effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Texture2D ringTex = ModContent.Request<Texture2D>("MogMod/Assets/Textures/VerticalSmearLarge").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Assets/Textures/GlowParticle").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColour = Projectile.GetAlpha(Color.Lerp(WeakColor, StrongColor, Timer)) * MathHelper.Min(1f, Timer);
            if (Projectile.timeLeft < 8) drawColour = Projectile.GetAlpha(Color.Lerp(drawColour, Color.Black, Timer * 0.5f)) * MathHelper.Min(1f, Timer * 0.5f);
            float rotation = MathHelper.PiOver2 - Main.GlobalTimeWrappedHourly * -(8f * Timer);

            for (int i = 0; i < 16; i++)
            {
                if (i % 2 == 0)
                {
                    Main.EntitySpriteDraw(ringTex, drawPosition, null, drawColour * 0.85f, rotation, ringTex.Size() * 0.5f, Projectile.scale * Timer * 0.075f, SpriteEffects.None);
                    Main.EntitySpriteDraw(ringTex, drawPosition, null, drawColour * 0.85f, rotation + MathHelper.PiOver2 + MathHelper.PiOver4, ringTex.Size() * 0.5f, Projectile.scale * Timer * 0.075f, SpriteEffects.None);
                    Main.EntitySpriteDraw(ringTex, drawPosition, null, drawColour * 0.85f, rotation + MathHelper.Pi + MathHelper.PiOver4, ringTex.Size() * 0.5f, Projectile.scale * Timer * 0.075f, SpriteEffects.None);
                }
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, drawColour * 0.75f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * ((Timer * 0.04f) * i), SpriteEffects.None);
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, drawColour * 0.5f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * ((Timer * 0.06f) * i), SpriteEffects.None);
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, drawColour * 0.35f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * ((Timer * 0.08f) * i), SpriteEffects.None);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}