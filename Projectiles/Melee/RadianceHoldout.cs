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
        //public ref float CurrentChargeMult => ref Projectile.ai[0];
        public float CurrentChargeMult = 0f;
        public ref float DustTimer => ref Projectile.ai[1];
        bool playedChargeSound = false;
        bool channelingBurn = false;
        bool canSwing = false;
        Color Color1 = Color.DarkGoldenrod;
        Color Color2 = Color.Firebrick;
        public SlotId AudSlot;
        public override void Defaults()
        {
            Projectile.extraUpdates = 3;
            swingWidth = 200;
            RotateInCooldown = 0f;
            RotateInStartup = 0f;
        }
        public override void Spawn()
        {
            StartupTime = 360;
            CooldownTime = 30;
            swingTime = 10;
            Projectile.timeLeft = 600;
            Projectile.scale *= 1.25f;
            CurrentChargeMult = Owner.MogMod().radiancePower;
            if (Main.mouseLeft && !canSwing) angle = new Vector2(0, 1);
        }
        /// <summary>
        /// if the player is channeling left click, hold radiance upwards and burn nearby enemies
        /// increase mogPlayer.radiancePower by 0.05f for each enemy it hits, up to a cap of 1f
        /// if the player is channeling left click && right clicks, stop holding upwards and swing
        /// continue swinging if the player is holding right click (refer to echo saber for continuous swinging)
        /// reduce mogPlayer.radiancePower by 0.05f for each swing
        /// 
        /// actually, make this left click swing and right click in startup to stop swing and burn enemies
        /// </summary>
        public override void AdditionalAI()
        {
            // so the player can stop swinging and still maintain charge
            Owner.MogMod().radiancePower = CurrentChargeMult;
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            var veloc = oldPlayerOffset - (Projectile.Center - Owner.Center);
            veloc.Normalize();
            // separate dust timer to account for projectile.extraUpdates
            DustTimer++;
            // keep proj alive
            Projectile.timeLeft++;
            // manage charge timer and right click attack
            if (inStartup && channelingBurn && timer > 30)
            {
                timer = StartupTime - 1;
            }
            // if the player is channeling left click, hold radiance upwards and burn nearby enemies
            if (Owner.channel)
            {
                if (!canSwing) channelingBurn = true;
                // if not swinging and in startup
                if (!canSwing && inStartup)
                {
                    CurrentChargeMult = timer / (float)(StartupTime - 1);

                    // if right clicking at (max charge OR can swing is true ? 0f)
                    if (CurrentChargeMult >= (canSwing ? 0f : 1f) && Owner.MogMod().mouseRight)
                    {
                        Main.NewText($"beggining swing, {CurrentChargeMult}", Color.LemonChiffon);
                        CurrentChargeMult -= 0.05f;
                        RotateInCooldown = 0.3f;
                        RotateInStartup = 0.3f;
                        canSwing = true;
                        channelingBurn = false;
                    }
                }
                if (DustTimer % Projectile.extraUpdates == 0)
                {
                    Vector2 dustVel = new Vector2(-60 * -Projectile.spriteDirection, -5).RotatedBy(Projectile.rotation + 0.7f * -Projectile.spriteDirection);
                    Vector2 spawnPos = Projectile.Center - dustVel.RotatedByRandom(0.4f) * Projectile.scale * CurrentChargeMult;

                    Dust dust2 = Dust.NewDustPerfect(spawnPos, DustID.DesertTorch, dustVel.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.5f, 1.4f) * 0.3f);
                    dust2.scale = Main.rand.NextFloat(1.45f, 1.95f) * CurrentChargeMult * Projectile.scale;
                    dust2.noGravity = true;
                    dust2.color = Main.rand.NextBool(3) ? Color2 : Color1;
                    dust2.fadeIn = CurrentChargeMult;

                    Dust dust3 = Dust.NewDustPerfect(spawnPos, DustID.FireworksRGB, dustVel.RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 0.6f) * 0.3f, 100, Main.rand.NextBool(3) ? Color2 : Color1, Main.rand.NextFloat(0.5f, 0.8f) * CurrentChargeMult * Projectile.scale);
                }
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                {
                    ChargeSound.Position = Projectile.Center;
                    ChargeSound.Pitch = Utils.Remap(CurrentChargeMult, 0, 1f, -0.4f, 0f);
                    ChargeSound.Volume = Utils.Remap(CurrentChargeMult, 0, 1f, 0f, 0.75f) * 100;
                }
                else if (timer != StartupTime - 1) AudSlot = SoundEngine.PlaySound(SoundID.DD2_EtherianPortalIdleLoop with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
            }
            if (inSwing)
            {
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();
                if (DustTimer % Projectile.extraUpdates == 0)
                {
                    float scale = Main.rand.NextFloat(0.5f, 1f);
                    var color = Main.rand.NextBool() ? Color1 : Color2;
                    if (CurrentChargeMult >= 1)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            float randRot = Main.rand.NextFloat(40, -50);
                            Vector2 dustVel = veloc.RotatedBy(MathHelper.PiOver4 * 0.5f * Projectile.spriteDirection) * Main.rand.NextFloat(2, 5);
                            Vector2 spawnPos = Projectile.Center + new Vector2(-angle.X.DirectionalSign(), Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation - 0.7f * Projectile.spriteDirection) * (randRot * (mogPlayer.swingNum % 2 == 0 ? -1f : 1f)) * Projectile.scale;
                            Dust dust2 = Dust.NewDustPerfect(spawnPos, DustID.AncientLight, -dustVel * Main.rand.NextFloat(0.4f, 0.7f), 100, Main.rand.NextBool(4) ? Color2 : Color1, Main.rand.NextFloat(0.3f, 0.35f) * Projectile.scale);
                        }
                        for (int i = 0; i < 6; i++)
                        {
                            float randRot = Main.rand.NextFloat(30, -45);
                            Vector2 dustVel = veloc.RotatedBy(MathHelper.PiOver4 * 0.5f * Projectile.spriteDirection) * Main.rand.NextFloat(2, 5);
                            Vector2 spawnPos = Projectile.Center + new Vector2(-angle.X.DirectionalSign(), Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation - 0.7f * Projectile.spriteDirection) * (randRot * (mogPlayer.swingNum % 2 == 0 ? -1f : 1f)) * Projectile.scale;
                            Dust dust2 = Dust.NewDustPerfect(spawnPos, DustID.FireworksRGB, dustVel * Main.rand.NextFloat(0.2f, 0.6f));
                            dust2.scale = Main.rand.NextFloat(1.35f, 1.85f) * Projectile.scale;
                            dust2.noGravity = true;
                            dust2.color = Main.rand.NextBool(3) ? Color2 : Color1;
                            dust2.fadeIn = Projectile.scale * 0.3f;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            float randRot = Main.rand.NextFloat(-40, -45);
                            Vector2 dustVel = veloc.RotatedBy(MathHelper.PiOver4 * 0.5f * Projectile.spriteDirection) * Main.rand.NextFloat(2, 5);
                            Vector2 spawnPos = Projectile.Center + new Vector2(-angle.X.DirectionalSign(), Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation - 0.7f * Projectile.spriteDirection) * (randRot * (mogPlayer.swingNum % 2 == 0 ? -1f : 1f)) * Projectile.scale;
                            Dust dust2 = Dust.NewDustPerfect(spawnPos, DustID.FireworksRGB, dustVel * Main.rand.NextFloat(0.1f, 0.5f));
                            dust2.scale = Main.rand.NextFloat(0.75f, 0.9f) * Projectile.scale;
                            dust2.noGravity = true;
                            dust2.color = Main.rand.NextBool(3) ? Color2 : Color1;
                            dust2.fadeIn = CurrentChargeMult;
                        }
                        if (Main.rand.NextBool(5)) scale *= 1.4f;
                        Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * MathHelper.Lerp(10, 30, Main.rand.NextFloat());
                        Dust d = Dust.NewDustPerfect(Projectile.Center + angle * 30, DustID.AncientLight, velocity, 100, color, scale);
                    }
                }
            }
            if (inCooldown && CooldownCompletion >= 1f && Owner.MogMod().mouseRight && canSwing)
            {
                Main.NewText($"resetting swing, {CurrentChargeMult}, {canSwing}");
                mogPlayer.swingNum++;
                timer = swingTimer = 0;
                Projectile.numHits = 0;
                Projectile.ResetLocalNPCHitImmunity();
                if (CurrentChargeMult <= 0f) canSwing = false;
            }
            Owner.heldProj = Projectile.whoAmI;
        }
        public override float SwingFunction()
        {
            if (channelingBurn) return 0f;
            if (inStartup) return MathHelper.ToRadians(MathHelper.SmoothStep(1f, -swingWidth * 0.66f, 1 - MathF.Pow(StartupCompletion, 0.5f))); // -swingWidth * 0.8f
            if (inCooldown) return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * 0.45f, swingWidth * 0.7f, MathF.Pow(CooldownCompletion, 0.5f)));
            return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * .66f, (swingWidth * 0.45f), MathF.Pow(SwingCompletion, 0.5f)));
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.SourceDamage *= CurrentChargeMult * 4.8f;
            modifiers.Knockback += CurrentChargeMult;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int time = 0;
            if (CurrentChargeMult >= 1)
            {
                if (Projectile.numHits == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost with { Volume = 1f, PitchVariance = 0.15f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { Volume = 0.9f, PitchVariance = 0.15f }, Projectile.Center);
                }
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact);
                SoundEngine.PlaySound(SoundID.Item69 with { Volume = 1f, LimitsArePerVariant = false, MaxInstances = 1 });
                time = 600;
            }
            else
            {
                if (Projectile.numHits == 0) SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce with { Volume = 0.85f, PitchVariance = 0.25f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack with { Volume = 0.5f, LimitsArePerVariant = false, MaxInstances = 1 });
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { VariantsWeights = new ReadOnlySpan<float>(new float[] { 1, 0, 0 }) });
                time = 300;
            }
            target.AddBuff(ModContent.BuffType<BlazingDebuff>(), time);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!inCooldown)
            {
                var tex = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/RadianceGhost").Value;
                float outlineWidth = (int)(4 * CurrentChargeMult) * 0.5f;
                if (inSwing)
                {
                    outlineWidth *= 1 - SwingCompletion;
                    Texture2D swoosh = ModContent.Request<Texture2D>("MogMod/Assets/Textures/VerticalSmearLarge").Value;
                    float rotation = Projectile.rotation - 0.7f * -Projectile.spriteDirection;
                    float rotationOffset = (Owner.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum % 2 == 0 ? MathHelper.PiOver4 : (MathHelper.TwoPi - MathHelper.PiOver4)) * (angle.X < 0 ? -1f : 1f);
                    Vector2 spawnPos = Projectile.Center + new Vector2(-angle.X.DirectionalSign(), 70f).RotatedBy(rotation) * Projectile.scale - Main.screenPosition;
                    Main.EntitySpriteDraw(swoosh, spawnPos, null, Color.Lerp(Color1, Color2, CurrentChargeMult) with { A = 0 } * SwingCompletion * (CurrentChargeMult * 0.75f), rotation + rotationOffset, swoosh.Size() * 0.5f, Projectile.scale * 0.35f, SpriteEffects.None);
                }
                for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.25f)
                {
                    Main.spriteBatch.Draw(
                        tex,
                        Projectile.Center + new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX.RotatedBy(i + Projectile.rotation) * outlineWidth * Projectile.scale - Main.screenPosition,
                        null,
                        Color.Lerp(Color1, Color2, CurrentChargeMult),
                        Projectile.rotation,
                        tex.Size() * 0.5f,
                        Projectile.scale,
                        Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                        0
                    );
                }
            }
            if (inSwing) return base.PreDraw(ref lightColor);
            return true;
        }
    }
}