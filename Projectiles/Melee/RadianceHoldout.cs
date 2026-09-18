using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
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
    public class RadianceHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public Player Owner => Main.player[Projectile.owner];
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<Radiance>()).Item;
        public override string Texture => BaseItem.ModItem.Texture;
        public override int AfterImageLength => 10;
        public override int OffsetDistance => 50;
        public override int CooldownTime { get; set; }
        public override SoundStyle? UseSound => SoundID.DD2_MonkStaffSwing with { Volume = 1f };
        public ref float DustTimer => ref Projectile.ai[1];
        float MaxCharge = 300f;
        float MinCharge = 0.5f;
        bool playedChargeSound = false;
        bool channelingBurn = false;
        Color Color1 = Color.DarkGoldenrod;
        Color Color2 = Color.Firebrick;
        public SlotId AudSlot;
        public override void Defaults()
        {
            Projectile.extraUpdates = 3;
            swingWidth = 200;
            RotateInCooldown = 0.3f;
            RotateInStartup = 0.3f;
        }
        public override void Spawn()
        {
            StartupTime = 30;
            CooldownTime = 30;
            swingTime = 10;
            Projectile.timeLeft = 600;
            Projectile.scale *= 1.25f;
        }
        public override void AdditionalAI()
        {
            Color color = Color.Lerp(Color1, Color2, Owner.MogMod().radiancePower);
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            if (channelingBurn)
            {
                DustTimer++;
                if (Projectile.owner == Main.myPlayer)
                {
                    var source = Projectile.GetSource_FromThis();
                    int damage = (int)(Owner.HeldItem.damage * 0.25f);
                    int type = ModContent.ProjectileType<RadianceAura>();
                    if (Owner.ownedProjectileCounts[type] < 1 && DustTimer % Projectile.extraUpdates == 0)
                    {
                        //Main.NewText($"summoning proj", Color.Green);
                        Projectile.NewProjectile(source, Owner.Center, Vector2.Zero, type, damage, 0f, Projectile.owner);
                    }
                    else
                    {
                        foreach (Projectile aura in Main.ActiveProjectiles)
                        {
                            if (aura.type == type) aura.timeLeft = 2;
                        }
                    }
                }
                if (Main.mouseLeft && timer == StartupTime - 1f)
                {
                    timer--;
                    Projectile.timeLeft++;
                    if (Owner.MogMod().radiancePower >= 1f)
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
                    Vector2 spawnPos = Projectile.Center - dustVel.RotatedByRandom(0.4f) * Projectile.scale * Owner.MogMod().radiancePower;

                    Dust dust2 = Dust.NewDustPerfect(spawnPos, DustID.DesertTorch, dustVel.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.5f, 1.4f) * 0.3f);
                    dust2.scale = Main.rand.NextFloat(1.45f, 1.95f) * Owner.MogMod().radiancePower * Projectile.scale;
                    dust2.noGravity = true;
                    dust2.color = color;
                    dust2.fadeIn = Owner.MogMod().radiancePower;

                    Dust dust3 = Dust.NewDustPerfect(spawnPos, DustID.FireworksRGB, dustVel.RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 0.6f) * 0.3f, 100, color, Main.rand.NextFloat(0.5f, 0.8f) * Owner.MogMod().radiancePower * Projectile.scale);
                }
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                {
                    ChargeSound.Position = Projectile.Center;
                    ChargeSound.Pitch = Utils.Remap(Owner.MogMod().radiancePower, 0, 1f, -0.4f, 0f);
                    ChargeSound.Volume = Utils.Remap(Owner.MogMod().radiancePower, 0, 1f, 0f, 0.75f) * 100;
                }
                else if (timer != StartupTime - 1) AudSlot = SoundEngine.PlaySound(SoundID.DD2_EtherianPortalIdleLoop with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
            }
            if (inStartup)
            {
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
            if (inCooldown && CooldownCompletion >= 1f && Main.mouseLeft)
            {
                mogPlayer.swingNum++;
                timer = swingTimer = 0;
                Projectile.timeLeft = 600;
                Projectile.numHits = 0;
                playedChargeSound = false;
                if (Owner.MogMod().radiancePower > 0f) Owner.MogMod().radiancePower -= 0.05f;
                Projectile.ResetLocalNPCHitImmunity();
            }
        }
        public override float SwingFunction()
        {
            if (channelingBurn) return 0f;
            if (inStartup) return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.8f, -swingWidth * 0.66f, 1 - MathF.Pow(StartupCompletion, 0.5f)));
            if (inCooldown) return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * 0.45f, swingWidth * 0.7f, MathF.Pow(CooldownCompletion, 0.5f)));
            return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * .66f, (swingWidth * 0.45f), MathF.Pow(SwingCompletion, 0.5f)));
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.SourceDamage *= (Owner.MogMod().radiancePower * 4f) + 0.5f;
            modifiers.Knockback += Owner.MogMod().radiancePower;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int time = (int)(300 * Owner.MogMod().radiancePower);
            if (Owner.MogMod().radiancePower >= MinCharge)
            {
                if (Projectile.numHits == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost with { Volume = 1f, PitchVariance = 0.15f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { Volume = 0.9f, PitchVariance = 0.15f }, Projectile.Center);
                }
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact);
                SoundEngine.PlaySound(SoundID.Item69 with { Volume = 1f, LimitsArePerVariant = false, MaxInstances = 1 });
            }
            else
            {
                if (Projectile.numHits == 0) SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce with { Volume = 0.85f, PitchVariance = 0.25f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack with { Volume = 0.5f, LimitsArePerVariant = false, MaxInstances = 1 });
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { VariantsWeights = new ReadOnlySpan<float>(new float[] { 1, 0, 0 }) });
            }
            target.AddBuff(ModContent.BuffType<BlazingDebuff>(), time);
        }
        public override void OnKill(int timeLeft) => base.OnKill(timeLeft);
        public override bool PreDraw(ref Color lightColor)
        {
            var tex = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/RadianceGhost").Value;
            float outlineWidth = (int)(4 * Owner.MogMod().radiancePower) * 0.5f;
            if (inSwing)
            {
                outlineWidth *= 1 - SwingCompletion;
                Texture2D swoosh = ModContent.Request<Texture2D>("MogMod/Assets/Textures/VerticalSmearLarge").Value;
                float rotation = Projectile.rotation - 0.7f * -Projectile.spriteDirection;
                float rotationOffset = (Owner.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum % 2 == 0 ? MathHelper.PiOver4 : (MathHelper.TwoPi - MathHelper.PiOver4)) * (angle.X < 0 ? -1f : 1f);
                Vector2 spawnPos = Projectile.Center + new Vector2(-angle.X.DirectionalSign(), 70f).RotatedBy(rotation) * Projectile.scale - Main.screenPosition;
                Main.EntitySpriteDraw(swoosh, spawnPos, null, Color.Lerp(Color1, Color2, Owner.MogMod().radiancePower) with { A = 0 } * SwingCompletion * (Owner.MogMod().radiancePower * 0.75f), rotation + rotationOffset, swoosh.Size() * 0.5f, Projectile.scale * 0.35f, SpriteEffects.None);
            }
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.25f)
            {
                Main.spriteBatch.Draw(
                    tex,
                    Projectile.Center + new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX.RotatedBy(i + Projectile.rotation) * outlineWidth * Projectile.scale - Main.screenPosition,
                    null,
                    Color.Lerp(Color1, Color2, Owner.MogMod().radiancePower),
                    Projectile.rotation,
                    tex.Size() * 0.5f,
                    Projectile.scale,
                    Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                    0
                );
            }
            if (inSwing) return base.PreDraw(ref lightColor);
            return true;
        }
    }
}