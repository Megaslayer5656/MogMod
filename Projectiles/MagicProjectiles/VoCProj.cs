using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Weapons.Magic;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class VoCProj : BaseHoldoutProjectile
    {
        public override LocalizedText DisplayName => MiscUtils.GetItemName<VortexOfConflagration>();
        public override float HoldoutOffset => Projectile.width;
        public override float TurnSpeed => 0.3f;
        public override int HoldoutHandling => HoldoutStyle.Floaty;
        public ref float Timer => ref Projectile.ai[0];
        public ref float CurrentChargeMult => ref Projectile.ai[1];
        public static Color Color1 => new(255, 179, 87);
        public static Color Color2 => new(87, 252, 255);
        public SlotId AudSlot;
        bool released = false;
        bool playedChargeSound = false;
        float MaxCharge = 1f;
        float MinCharge = 0.25f;
        float radius = 16f;
        int debuffTime = 360;
        int maxHits = 22;
        int Cap = 30;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.alpha = 255;
            Projectile.hide = true;

            Projectile.netImportant = true;
        }
        public override void HoldoutAI()
        {
            Timer++;
            if (Timer == 3) Projectile.alpha = 0;

            var attackSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
            if (attackSpeed > Cap) attackSpeed = Cap;
            if (attackSpeed != 0f) attackSpeed = 1f / attackSpeed;

            bool canUseMana = Owner.CheckMana(Owner.HeldItem);
            if (Owner.CantUseHoldout() || released)
            {
                if (!released && CurrentChargeMult >= MinCharge)
                {
                    Release();
                    return;
                }
                else if (!canUseMana || CurrentChargeMult < MinCharge)
                {
                    Projectile.Kill();
                    return;
                }
            }
            else
            {
                if (!canUseMana)
                {
                    if (CurrentChargeMult >= MinCharge) Release();
                    released = true;
                    return;
                }
                if (Timer % 4 == 0f)
                {
                    Owner.CheckMana(Owner.HeldItem, -1, true);
                    if (CurrentChargeMult < MaxCharge) CurrentChargeMult += (0.02f / attackSpeed);
                }
                // While channeled, keep refreshing the projectile lifespan
                Projectile.timeLeft = 2;
                maxHits = (int)(CurrentChargeMult * 22f);
                if (CurrentChargeMult >= MinCharge)
                {
                    int dustNum = (int)MathHelper.Clamp(CurrentChargeMult * 12f, 3f, 12f);
                    for (int s = 0; s < dustNum; s++)
                    {
                        float dustRot = Main.GlobalTimeWrappedHourly * -5.75f + (MathHelper.TwoPi / dustNum * s);
                        Vector2 dustPos = Projectile.Center + Vector2.UnitX.RotatedBy(dustRot) * 25f * CurrentChargeMult;
                        Vector2 dustVel = Vector2.Normalize(dustPos - Projectile.Center).RotatedBy(MathHelper.ToRadians(70)) * 2f * (CurrentChargeMult * 1.5f);
                        Dust d = Dust.NewDustPerfect(dustPos, DustID.FireworksRGB, dustVel, 100, Color.Lerp(Color2, Color1, CurrentChargeMult));
                        d.noGravity = true;
                        d.velocity *= 1.4f;
                    }
                    if (CurrentChargeMult >= MaxCharge)
                    {
                        if (!playedChargeSound)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost with { Pitch = 0.3f }, Projectile.Center);
                            SoundEngine.PlaySound(SoundID.Item84, Projectile.Center);
                            playedChargeSound = true;
                        }
                    }
                }
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                {
                    ChargeSound.Position = Projectile.Center;
                    ChargeSound.Pitch = Utils.Remap(CurrentChargeMult, 0, 1f, -0.4f, 0f);
                    ChargeSound.Volume = Utils.Remap(CurrentChargeMult, 0, 1f, 0f, 0.75f) * 100;
                }
                else if (CurrentChargeMult < MinCharge) AudSlot = SoundEngine.PlaySound(SoundID.DD2_EtherianPortalIdleLoop with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
            }
        }
        public void Release()
        {
            float speed = 25f * CurrentChargeMult;
            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * speed;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 360;
            Projectile.velocity = shootVelocity;
            Owner.channel = false;
            SoundEngine.PlaySound(SoundID.DD2_BetsysWrathShot with { Pitch = (CurrentChargeMult + 1f) * 0.15f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { VariantsWeights = new ReadOnlySpan<float>(new float[] { 1, 0, 0 }) });
            if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();

            int dustNum = (int)MathHelper.Clamp(CurrentChargeMult * 25, 5, 25);
            for (int i = 0; i < dustNum; i++)
            {
                float variance = Main.rand.NextFloat(-0.5f, 0.5f);
                int dustType = 278;
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, dustType, Projectile.velocity);
                dust2.scale = Main.rand.NextFloat(1.2f, 1.4f) - Math.Abs(variance);
                dust2.velocity = -shootVelocity.RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance));
                dust2.noGravity = true;
                dust2.color = Color.Lerp(Color2, Color1, CurrentChargeMult);
            }
            released = true;
        }
        public override void AI()
        {
            // if the proj is released, behave like a regular homing projectile
            if (released)
            {
                Projectile.localNPCHitCooldown = 10;
                released = true;
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();
                MogModUtils.HomeInOnNPC(Projectile, false, 500f, 20f, 35f);
                for (int i = 0; i < 2; i++)
                {
                    float velocityX = Projectile.velocity.X / 3f * (float)i;
                    float velocityY = Projectile.velocity.Y / 3f * (float)i;
                    int waterFlame = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f, 100, Color.Lerp(Color1, Color2, CurrentChargeMult), 1.2f);
                    Dust dust = Main.dust[waterFlame];
                    dust.noGravity = true;
                    dust.velocity *= 0.1f;
                    dust.velocity += Projectile.velocity * 0.1f;
                    dust.position.X -= velocityX;
                    dust.position.Y -= velocityY;
                }
                if (Main.rand.NextBool(10))
                {
                    int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? DustID.FireworksRGB : 176, 0f, 0f, 100, Color.Lerp(Color2, Color1, CurrentChargeMult), 0.6f);
                    Main.dust[dust2].velocity *= 0.25f;
                    Main.dust[dust2].velocity += Projectile.velocity * 0.5f;
                }
                if (Projectile.numHits > maxHits) Projectile.Kill();
                return;
            }
            // otherwise, use holdoutAI
            base.AI();
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);
        public override bool? CanDamage() => CurrentChargeMult >= MinCharge;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!released) return false;
            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;
            Projectile.numHits++;
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => MogModUtils.CircularHitboxCollision(Projectile.Center, radius * (CurrentChargeMult + 1f), targetHitbox);
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= released ? (CurrentChargeMult * 2f) : CurrentChargeMult;
            modifiers.Knockback += released ? CurrentChargeMult : 0f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!released) Projectile.numHits--;
            target.AddBuff(BuffID.Daybreak, (int)(debuffTime * (released ? CurrentChargeMult : MinCharge)));
            target.AddBuff(BuffID.Wet, (int)(debuffTime * (released ? CurrentChargeMult : MinCharge)));
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Daybreak, (int)(debuffTime * (released ? CurrentChargeMult : MinCharge)));
            target.AddBuff(BuffID.Wet, (int)(debuffTime * (released ? CurrentChargeMult : MinCharge)));
        }
        public override void OnKill(int timeLeft)
        {
            if (CurrentChargeMult < MinCharge) return;
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            for (int k = 0; k < 15; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.AncientLight, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, 100, Color.Lerp(Color1, Color2, CurrentChargeMult));
            for (int i = 0; i < 9; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.FireworksRGB, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, 100, Color.Lerp(Color2, Color1, CurrentChargeMult), 1.7f);
                Main.dust[dust].velocity *= 1.4f;
            }
        }
        // custom drawing to apply custom rotation and glow over the texture
        public override bool PreDraw(ref Color lightColor)
        {
            // draw original proj
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Projectile.GetAlpha(lightColor);
            float rotation = MathHelper.PiOver2 - Main.GlobalTimeWrappedHourly * 4f * (CurrentChargeMult + 1f);
            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, -(rotation * Projectile.direction), texture.Size() * 0.5f, Projectile.scale * CurrentChargeMult, SpriteEffects.None);

            // draw glow effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Texture2D ringTex = ModContent.Request<Texture2D>("MogMod/Assets/Textures/GlowRingParticle").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Assets/Textures/GlowParticle").Value;
            for (int i = 0; i < 4; i++)
            {
                Color drawColour = Projectile.GetAlpha(Color.Lerp(Color2, Color1, CurrentChargeMult));
                Main.EntitySpriteDraw(ringTex, drawPosition, null, drawColour * 0.85f, Projectile.rotation, ringTex.Size() * 0.5f, Projectile.scale * ((CurrentChargeMult * 0.18f) * i), SpriteEffects.None);
                Main.EntitySpriteDraw(ringTex, drawPosition, null, drawColour * 0.25f, Projectile.rotation, ringTex.Size() * 0.5f, Projectile.scale * ((CurrentChargeMult * 0.19f) * i), SpriteEffects.None);
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, drawColour * 0.35f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * ((CurrentChargeMult * 0.35f) * i), SpriteEffects.None);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}