using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class FlamewallHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public Player Owner => Main.player[Projectile.owner];
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<Flamewall>()).Item;
        public override string Texture => BaseItem.ModItem.Texture;
        public override int OffsetDistance => 90;
        public override int CooldownTime { get; set; }
        public override SoundStyle? UseSound => SoundID.Item105 with { Volume = MathHelper.Lerp(0.6f, 1f, FlamewallPower), Pitch = -FlamewallPower * 0.5f, PitchVariance = 0.1f };
        //public override bool AlternateSwings => false;
        public ref float DustTimer => ref Projectile.ai[1];
        public float FlamewallPower => Owner.MogMod().flamewallPower;
        float MaxCharge = 300f;
        float MinCharge = 0.5f;
        int shootCooldown = 60;
        int numHits = 0;
        bool playedChargeSound = false;
        bool playedSwingSound = false;
        bool channelingBurn = false;
        public static Color WeakColor => Flamewall.WeakColor;
        public static Color StrongColor => Flamewall.StrongColor;
        public SlotId AudSlot;
        public override void Defaults()
        {
            Projectile.extraUpdates = 5;
            swingWidth = 900;
            RotateInCooldown = 0.3f;
            RotateInStartup = 0.3f;
        }
        public override void Spawn()
        {
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            mogPlayer.swingNum = (mogPlayer.swingNum + 1) % 2;
            StartupTime = 40;
            CooldownTime = 20;
            swingTime = 40;
            AfterImageLength = 10;
            Projectile.timeLeft = 600;
            Projectile.scale *= 1.25f;
        }
        public override void AdditionalAI()
        {
            Color color = Color.Lerp(WeakColor, StrongColor, FlamewallPower);
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            if (channelingBurn)
            {
                Owner.MogMod().flamewallPower = 1f;
                AfterImageLength = 0;
                DustTimer++;
                angle = new Vector2((Owner.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX).X > 0 ? 0.000001f : -0.000001f, MathHelper.Lerp(1f, 1.15f, FlamewallPower));
                if (Projectile.owner == Main.myPlayer)
                {
                    var source = Projectile.GetSource_FromThis();
                    int type = ModContent.ProjectileType<FlamewallBolt>();
                    if (DustTimer % (shootCooldown * Projectile.extraUpdates) == 0)
                    {
                        NPC target = Projectile.Center.ClosestNPCAt(1000);
                        if (target != null) MogModUtils.MagnetSphereHitscan(Projectile, Vector2.Distance(Projectile.Center, target.Center), 8f, 0, 1, type);
                    }
                }
                if (Main.mouseLeft && timer == StartupTime - 1f)
                {
                    timer--;
                    Projectile.timeLeft++;
                    if (FlamewallPower >= 1f)
                    {
                        if (!playedChargeSound)
                        {
                            SoundEngine.PlaySound(SoundID.DeerclopsStep with { Volume = 2f, Pitch = 0.5f }, Projectile.Center);
                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { Volume = 2f, Pitch = -0.2f }, Projectile.Center);
                            playedChargeSound = true;
                            for (int i = 0; i < 5; i++)
                            {
                                float scale = Main.rand.NextFloat(0.5f, 1f);

                                if (Main.rand.NextBool(5)) scale *= 1.4f;
                                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * MathHelper.Lerp(10, 30, Main.rand.NextFloat());
                                Dust d = Dust.NewDustPerfect(Projectile.Center + angle * 30, DustID.AncientLight, velocity, 100, color, scale);
                            }
                        }
                    }
                }
                if (DustTimer % Projectile.extraUpdates == 0)
                {
                    Vector2 dustVel = new Vector2(-60 * -Projectile.spriteDirection, -5).RotatedBy(Projectile.rotation + 0.7f * -Projectile.spriteDirection);
                    Vector2 spawnPos = Projectile.Center - dustVel.RotatedByRandom(0.4f) * Projectile.scale * FlamewallPower;

                    Dust dust2 = Dust.NewDustPerfect(spawnPos, DustID.DesertTorch, dustVel.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.5f, 1.4f) * 0.3f);
                    dust2.scale = Main.rand.NextFloat(1.45f, 1.95f) * FlamewallPower * Projectile.scale;
                    dust2.noGravity = true;
                    dust2.color = color;
                    dust2.fadeIn = FlamewallPower;

                    Dust dust3 = Dust.NewDustPerfect(spawnPos, DustID.FireworksRGB, dustVel.RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 0.6f) * 0.3f, 100, color, Main.rand.NextFloat(0.5f, 0.8f) * FlamewallPower * Projectile.scale);
                }
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                {
                    ChargeSound.Position = Projectile.Center;
                    ChargeSound.Pitch = Utils.Remap(FlamewallPower, 0, 1f, -0.4f, 0f);
                    ChargeSound.Volume = Utils.Remap(FlamewallPower, 0, 1f, 0f, 0.75f) * 100;
                }
                else if (timer != StartupTime - 1) AudSlot = SoundEngine.PlaySound(SoundID.DD2_EtherianPortalIdleLoop with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
            }
            if (inStartup)
            {
                AfterImageLength = 0;
                if (Main.mouseLeft && Owner.MogMod().mouseRight)
                {
                    if (!channelingBurn)
                    {
                        angle = new Vector2(0f, 1f);
                        RotateInCooldown = 0f;
                        RotateInStartup = 0f;
                        channelingBurn = true;
                    }
                }
                else
                {
                    if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();
                    if (channelingBurn)
                    {
                        Projectile.timeLeft = 600;
                        timer = swingTimer = 0;
                        RotateInCooldown = 0.3f;
                        RotateInStartup = 0.3f;
                        channelingBurn = false;
                    }
                }
            }
            if (inSwing)
            {
                if (!playedSwingSound)
                {
                    SoundEngine.PlaySound(SoundID.Item70 with { Volume = 2f * FlamewallPower, Pitch = -0.3f * FlamewallPower }, Projectile.Center);
                    playedSwingSound = true;
                }
                
                AfterImageLength = 10;
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();
                var veloc = oldPlayerOffset - (Projectile.Center - Main.player[Projectile.owner].Center);
                veloc.Normalize();
                float maxRotationDeviance = 0.8f;
                float rotationAngle = Main.rand.NextFloat(-maxRotationDeviance, maxRotationDeviance);
                float scale = Main.rand.NextFloat(0.7f, 1.15f);
                Vector2 velocity = veloc.RotatedBy(MathHelper.PiOver4 * 0.5f * Projectile.spriteDirection) * Main.rand.NextFloat(1, 4);

                for (int i = 0; i < 1; i++)
                {
                    Dust outerDust = Dust.NewDustPerfect(Projectile.Center + new Vector2(-angle.X.DirectionalSign(), Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation - 0.7f * Projectile.spriteDirection) * (Main.rand.NextFloat(-30, -45) * (mogPlayer.swingNum % 2 == 0 ? -1f : 1f)) * Projectile.scale, DustID.FireworksRGB, velocity, 100, Color.WhiteSmoke, scale);
                    outerDust.scale *= Main.rand.NextFloat(0.75f, 1.05f);
                    if (Main.rand.NextBool(4)) outerDust.scale *= Main.rand.NextFloat(0.25f, 1.65f);
                    outerDust.noGravity = true;
                    outerDust.color = color;
                }
            }
            if (inSwing && swingTimer % (swingTime / 3) == (swingTime / 3) - 1 && numHits < 2)
            {
                Projectile.ResetLocalNPCHitImmunity();
                numHits++;
            }
            if (inCooldown && CooldownCompletion >= 1f && Main.mouseLeft)
            {
                timer = swingTimer = 0;
                Projectile.timeLeft = ExistsTime * 2;
                Projectile.numHits = numHits = 0;
                playedChargeSound = playedSwingSound = false;
                Projectile.ResetLocalNPCHitImmunity();
            }
        }
        public override float SwingFunction()
        {
            if (channelingBurn)
            {
                if (FlamewallPower >= 1f) return Main.rand.NextFloat(-0.025f, 0.025f);
                else if (FlamewallPower >= MinCharge) return Main.rand.NextFloat(-0.01f, 0.01f);

                return 0f;
            }
            if (inStartup) return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.45f, -swingWidth * 0.66f, MathF.Pow(StartupCompletion, 0.5f)));
            if (inCooldown) return MathHelper.ToRadians(MathHelper.Lerp(swingWidth * 0.8f, swingWidth * 0.75f, 1 - MathF.Pow(1 - CooldownCompletion, 0.5f)));
            return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.66f, swingWidth * 0.8f, SwingCompletion));
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int time = (int)(480 * FlamewallPower);
            Vector2 launchVel = Utils.DirectionTo(Owner.Center, Owner.MogMod().mouseWorld);
            int dustNum = (int)MathHelper.Clamp(FlamewallPower * 25, 5, 25);
            for (int i = 0; i < dustNum; i++)
            {
                float variance = Main.rand.NextFloat(-0.5f, 0.5f);
                int dustStyle = 278;
                Dust dust2 = Dust.NewDustPerfect(target.Center, dustStyle, Projectile.velocity);
                dust2.scale = Main.rand.NextFloat(1.2f, 1.4f) - Math.Abs(variance);
                dust2.velocity = (launchVel * 25).RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance));
                dust2.noGravity = true;
                dust2.color = Color.Lerp(WeakColor, StrongColor, FlamewallPower);
            }
            if (numHits < 3)
            {
                if (Projectile.owner == Main.myPlayer) if (FlamewallPower > 0f) Owner.MogMod().flamewallPower -= 0.025f;
                if (FlamewallPower > 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce with { Volume = FlamewallPower * 1.5f, PitchVariance = FlamewallPower * 0.25f, MaxInstances = 3 }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { MaxInstances = 3 });
                    if (FlamewallPower > MinCharge * 0.5f)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost with { Volume = 1f, Pitch = -FlamewallPower * 0.1f, PitchVariance = 0.15f, MaxInstances = 3 }, Projectile.Center);
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { VariantsWeights = new ReadOnlySpan<float>(new float[] { 1, 0, 0 }) });
                    }
                    if (FlamewallPower >= MinCharge)
                    {
                        SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack with { Volume = FlamewallPower * 0.75f, LimitsArePerVariant = false, MaxInstances = 1 });
                        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { Volume = 0.9f, Pitch = -FlamewallPower * 0.1f, PitchVariance = 0.15f, MaxInstances = 3 }, Projectile.Center);
                        SoundEngine.PlaySound(SoundID.Item69 with { Volume = 1f, LimitsArePerVariant = false, MaxInstances = 1 });
                    }
                }
            }
            target.AddBuff(ModContent.BuffType<InfernoDebuff>(), time);
        }
        public override void OnKill(int timeLeft) => base.OnKill(timeLeft);
        public override bool PreDraw(ref Color lightColor)
        {
            var tex = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/FlamewallGhost").Value;
            float outlineWidth = (int)(6 * FlamewallPower) * 0.5f;
            if (inSwing || inCooldown)
            {
                outlineWidth *= 1 - SwingCompletion;
                Texture2D swoosh = ModContent.Request<Texture2D>("MogMod/Assets/Textures/VerticalSmearLarge").Value;
                float rotation = Projectile.rotation - 0.7f * -Projectile.spriteDirection;
                float rotationOffset = (Owner.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum % 2 == 0 ? MathHelper.PiOver4 : (MathHelper.TwoPi - MathHelper.PiOver4)) * (angle.X < 0 ? -1f : 1f);
                Vector2 spawnPos = Projectile.Center + new Vector2(-angle.X.DirectionalSign(), 130f).RotatedBy(rotation) * Projectile.scale - Main.screenPosition;
                float fadeIn = Math.Min(1f, Math.Clamp(1f - (CooldownCompletion + 0.75f), 0f, 1f));
                Main.EntitySpriteDraw(swoosh, spawnPos, null, Color.Lerp(WeakColor, StrongColor, FlamewallPower) with { A = 0 } * (SwingCompletion * (FlamewallPower * 0.75f)) * (SwingCompletion >= 0.25f ? fadeIn : 1f), rotation + rotationOffset, swoosh.Size() * 0.5f, Projectile.scale * 0.7f, SpriteEffects.None);
            }
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.25f)
            {
                Main.spriteBatch.Draw(
                    tex,
                    Projectile.Center + new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX.RotatedBy(i + Projectile.rotation) * outlineWidth * Projectile.scale - Main.screenPosition,
                    null,
                    Color.Lerp(WeakColor, StrongColor, FlamewallPower),
                    Projectile.rotation,
                    tex.Size() * 0.5f,
                    Projectile.scale,
                    Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                    0
                );
            }
            return base.PreDraw(ref lightColor);
        }
    }
}