using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using ReLogic.Utilities;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class AstralCataclysmHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public Player Owner => Main.player[Projectile.owner];
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<AstralCataclysm>()).Item;
        public override string Texture => BaseItem.ModItem.Texture;
        public override int AfterImageLength => 10;
        public override int OffsetDistance => 80;
        public override int CooldownTime { get; set; }
        public override SoundStyle? UseSound => SoundID.DD2_SonicBoomBladeSlash with { Volume = 1f, Pitch = 0.5f, PitchVariance = 0.3f };
        public ref float CurrentChargeMult => ref Projectile.ai[0];
        public ref float DustTimer => ref Projectile.ai[1];
        bool playedChargeSound = false;
        bool playedSwingSound = false;
        Color Color1 = AstralCataclysm.MainColor1;
        Color Color2 = AstralCataclysm.MainColor2;
        Color Color3 = AstralCataclysm.MainColor3;
        public SlotId AudSlot;
        public static readonly Color[] colorList =
        [
            AstralCataclysm.MainColor2,
            AstralCataclysm.MainColor1,
            AstralCataclysm.MainColor3
        ];
        public override void Defaults()
        {
            Projectile.width = 92;
            Projectile.height = 110;
            Projectile.extraUpdates = 3;
            swingWidth = 200;
            RotateInCooldown = 0.3f;
            RotateInStartup = 0.3f;
        }
        public override void Spawn()
        {
            StartupTime = Main.zenithWorld ? 360 : 60;
            CooldownTime = 30;
            swingTime = 10;
            Projectile.timeLeft = 600;
            Projectile.scale *= Main.zenithWorld ? 5f : 2f;
        }
        public override void AdditionalAI()
        {
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            var veloc = oldPlayerOffset - (Projectile.Center - Owner.Center);
            veloc.Normalize();
            Color dustColor = Main.rand.NextBool(3) ? Color3 : Main.rand.NextBool() ? Color2 : Color1;
            DustTimer++;
            if (inStartup)
            {
                CurrentChargeMult = timer / (float)(StartupTime - 1);
                if (DustTimer % Projectile.extraUpdates == 0)
                {
                    Vector2 dustVel = new Vector2(-60 * -Projectile.spriteDirection, -5).RotatedBy(Projectile.rotation + 0.7f * -Projectile.spriteDirection);
                    Vector2 spawnPos = Projectile.Center - dustVel.RotatedByRandom(0.4f) * Projectile.scale * CurrentChargeMult;

                    Dust dust2 = Dust.NewDustPerfect(spawnPos, DustID.RainbowTorch, dustVel.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.5f, 1.4f) * 0.3f);
                    dust2.scale = Main.rand.NextFloat(1.45f, 1.95f) * CurrentChargeMult * Projectile.scale;
                    dust2.noGravity = true;
                    dust2.color = dustColor;
                    dust2.fadeIn = CurrentChargeMult;

                    Dust dust3 = Dust.NewDustPerfect(spawnPos, DustID.FireworksRGB, dustVel.RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 0.6f) * 0.3f, 100, dustColor, Main.rand.NextFloat(0.5f, 0.8f) * CurrentChargeMult * Projectile.scale);
                }
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                {
                    ChargeSound.Position = Projectile.Center;
                    ChargeSound.Pitch = Utils.Remap(CurrentChargeMult, 0, 1f, 0f, 0.4f);
                    ChargeSound.Volume = Utils.Remap(CurrentChargeMult, 0, 1f, 0f, 0.75f) * 100;
                }
                else if (timer != StartupTime - 1) AudSlot = SoundEngine.PlaySound(SoundID.DD2_EtherianPortalIdleLoop with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
            }
            if (inStartup && !Owner.channel && timer > 30) timer = StartupTime - 1;
            if (Owner.channel)
            {
                if (timer == StartupTime - 1)
                {
                    Projectile.timeLeft++;
                    timer--;
                    if (!playedChargeSound)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot with { Volume = 2f, Pitch = -0.5f }, Projectile.Center);
                        playedChargeSound = true;
                        for (int i = 0; i < 5; i++)
                        {
                            float scale = Main.rand.NextFloat(0.5f, 1f);

                            if (Main.rand.NextBool(5)) scale *= 1.4f;
                            Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * MathHelper.Lerp(10, 30, Main.rand.NextFloat());
                            Dust d = Dust.NewDustPerfect(Projectile.Center + angle * 30, DustID.AncientLight, velocity, 100, dustColor, scale);
                        }
                    }
                }
            }
            if (inSwing)
            {
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();
                if (!playedSwingSound)
                {
                    SoundEngine.PlaySound(SoundID.Item15 with { Volume = 0.65f, Pitch = 0.4f, PitchVariance = 0.15f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack with { Pitch = 0.9f, PitchVariance = 0.15f }, Projectile.Center);
                    playedSwingSound = true;
                }
                var type = ModContent.ProjectileType<AstralStar>();
                float size = 2.5f;
                Rectangle swordBox = new((int)(Projectile.Center.X - Projectile.width * size / 2), (int)(Projectile.Center.Y - Projectile.height * size / 2), (int)(Projectile.Hitbox.Width * size), (int)(Projectile.Hitbox.Height * size));
                if (Projectile.owner == Main.myPlayer) foreach (Projectile star in Main.projectile.Where(star => star.active && star.damage > 0 && Projectile.Colliding(swordBox, star.Hitbox)))
                {
                    Vector2 AimVelocity(float velocity) => (star.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * velocity;
                    if (star.type == type && star.timeLeft < AstralStar.Lifetime)
                    { 
                        if (CurrentChargeMult >= 1)
                        {
                            star.velocity += AimVelocity(-25f);
                            star.timeLeft = AstralStar.Lifetime + 20;
                            star.numHits++;
                            star.owner = Owner.whoAmI;
                            star.netUpdate = true;
                            star.ai[1] = 1f;
                            SoundEngine.PlaySound(SoundID.Item4 with { Pitch = 0.8f, PitchVariance = 0.4f }, Projectile.Center);
                            SoundEngine.PlaySound(SoundID.Item37 with { Pitch = 0.9f, PitchVariance = 0.1f }, Projectile.Center);
                        }
                        else
                        {
                            star.velocity += AimVelocity(-15f * CurrentChargeMult);
                            star.timeLeft = AstralStar.Lifetime + 20;
                            star.numHits++;
                            star.owner = Owner.whoAmI;
                            star.netUpdate = true;
                            star.ai[1] = 2f;
                            SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact with { Pitch = 0.8f, PitchVariance = 0.4f }, Projectile.Center);
                            SoundEngine.PlaySound(SoundID.Item37 with { Volume = 0.65f, Pitch = 0.9f, PitchVariance = 0.1f }, Projectile.Center);
                        }
                    }
                }
                if (DustTimer % Projectile.extraUpdates == 0)
                {
                    float scale = Main.rand.NextFloat(0.5f, 1f);
                    if (CurrentChargeMult >= 1)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            float randRot = Main.rand.NextFloat(40, -65);
                            Vector2 dustVel = veloc.RotatedBy(MathHelper.PiOver4 * 0.5f * Projectile.spriteDirection) * Main.rand.NextFloat(2, 5);
                            Vector2 spawnPos = Projectile.Center + new Vector2(-angle.X.DirectionalSign(), Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation - 0.7f * Projectile.spriteDirection) * (randRot * (mogPlayer.swingNum % 2 == 0 ? -1f : 1f)) * Projectile.scale;
                            Dust dust2 = Dust.NewDustPerfect(spawnPos, DustID.AncientLight, -dustVel * Main.rand.NextFloat(0.4f, 0.7f), 100, Main.rand.NextBool(4) ? Color2 : Color1, Main.rand.NextFloat(0.3f, 0.35f) * Projectile.scale);
                        }
                        for (int i = 0; i < 6; i++)
                        {
                            float randRot = Main.rand.NextFloat(30, -60);
                            Vector2 dustVel = veloc.RotatedBy(MathHelper.PiOver4 * 0.5f * Projectile.spriteDirection) * Main.rand.NextFloat(2, 5);
                            Vector2 spawnPos = Projectile.Center + new Vector2(-angle.X.DirectionalSign(), Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation - 0.7f * Projectile.spriteDirection) * (randRot * (mogPlayer.swingNum % 2 == 0 ? -1f : 1f)) * Projectile.scale;
                            Dust dust2 = Dust.NewDustPerfect(spawnPos, DustID.FireworksRGB, dustVel * Main.rand.NextFloat(0.2f, 0.6f));
                            dust2.scale = Main.rand.NextFloat(1.35f, 1.85f) * Projectile.scale;
                            dust2.noGravity = true;
                            dust2.color = Main.rand.NextBool(4) ? Color2 : Color3;
                            dust2.fadeIn = Projectile.scale * 0.3f;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            float randRot = Main.rand.NextFloat(-40, -65);
                            Vector2 dustVel = veloc.RotatedBy(MathHelper.PiOver4 * 0.5f * Projectile.spriteDirection) * Main.rand.NextFloat(2, 5);
                            Vector2 spawnPos = Projectile.Center + new Vector2(-angle.X.DirectionalSign(), Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation - 0.7f * Projectile.spriteDirection) * (randRot * (mogPlayer.swingNum % 2 == 0 ? -1f : 1f)) * Projectile.scale;
                            Dust dust2 = Dust.NewDustPerfect(spawnPos, DustID.FireworksRGB, dustVel * Main.rand.NextFloat(0.1f, 0.5f));
                            dust2.scale = Main.rand.NextFloat(0.75f, 0.9f) * Projectile.scale;
                            dust2.noGravity = true;
                            dust2.color = dustColor;
                            dust2.fadeIn = CurrentChargeMult;
                        }
                        if (Main.rand.NextBool(5)) scale *= 1.4f;
                        Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * MathHelper.Lerp(10, 30, Main.rand.NextFloat());
                        Dust d = Dust.NewDustPerfect(Projectile.Center + angle * 30, DustID.AncientLight, velocity, 100, dustColor, scale);
                    }
                }
            }
            Owner.heldProj = Projectile.whoAmI;
        }
        public override float SwingFunction()
        {
            if (inStartup) return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.8f, -swingWidth * 0.66f, 1 - MathF.Pow(StartupCompletion, 0.5f)));
            if (inCooldown) return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * 0.45f, swingWidth * 0.7f, MathF.Pow(CooldownCompletion, 0.5f)));
            return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * .66f, (swingWidth * 0.45f), MathF.Pow(SwingCompletion, 0.5f)));
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.SourceDamage *= CurrentChargeMult * 4.8f;
            modifiers.Knockback += (CurrentChargeMult);
            if (target.life >= (int)(target.lifeMax * 0.9f)) modifiers.FinalDamage *= 1.5f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            var type = ModContent.ProjectileType<AstralStar>();
            Vector2 aimVel(float velocity) => (Owner.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * velocity;
            int Damage(float mult = 1f) => (int)(Projectile.damage * mult);
            if (CurrentChargeMult >= 1)
            {
                if (Projectile.numHits == 0)
                {
                    for (float i = -0.5f; i < 0.26f; i += 0.25f)
                    {
                        if (Projectile.owner == Main.myPlayer) Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, -(aimVel(50f) / 4).RotatedByRandom(i) * Main.rand.NextFloat(0.6f, 1.4f), type, Damage(), Projectile.knockBack, Projectile.owner);

                        Dust chargefull = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, newColor: Main.rand.NextBool() ? Color1 : Color2);
                        Vector2 vel = -(aimVel(30f) / 4).RotatedByRandom(i) * Main.rand.NextFloat(0.6f, 1.4f);
                        Dust dust2 = Dust.NewDustPerfect(target.Center, DustID.FireworksRGB, vel, 80, Color2, 1.2f * Projectile.scale);
                    }
                    SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost with { Volume = 1f, PitchVariance = 0.15f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { Volume = 0.9f, PitchVariance = 0.15f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact with { Volume = 0.9f, PitchVariance = 0.15f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot);
                    SoundEngine.PlaySound(SoundID.Item69 with { Volume = 1f, LimitsArePerVariant = false, MaxInstances = 1 });
                }
            }
            else
            {
                if (Projectile.numHits == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost with { Volume = 0.5f, LimitsArePerVariant = false, MaxInstances = 1 });
                    SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce with { Volume = 0.85f, PitchVariance = 0.25f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { VariantsWeights = new ReadOnlySpan<float>(new float[] { 1, 0, 0 }) });
                    if (CurrentChargeMult >= 0.5f)
                    {
                        for (float i = -0.25f; i < 0.26f; i += 0.5f)
                        {
                            if (Projectile.owner == Main.myPlayer) Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, -(aimVel(40f * CurrentChargeMult) / 4).RotatedByRandom(i) * Main.rand.NextFloat(0.6f, 1.4f), type, Damage(0.75f), Projectile.knockBack, Projectile.owner);
                        }
                    }
                    else
                    {
                        if (CurrentChargeMult < 0.5f)
                        {
                            if (Projectile.owner == Main.myPlayer) Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, -(aimVel(30f * CurrentChargeMult) / 4).RotatedByRandom(CurrentChargeMult) * Main.rand.NextFloat(0.6f, 1.4f), type, Damage(0.5f), Projectile.knockBack, Projectile.owner);
                        }
                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!inCooldown)
            {
                var tex = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/AstralCataclysmGhost").Value;
                float outlineWidth = (int)(4 * CurrentChargeMult) * 0.5f;
                if (inSwing)
                {
                    outlineWidth *= 1 - SwingCompletion;
                    Texture2D swoosh = ModContent.Request<Texture2D>("MogMod/Assets/Textures/VerticalSmearLarge").Value;
                    float rotation = Projectile.rotation - 0.7f * -Projectile.spriteDirection;
                    float rotationOffset = (Owner.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum % 2 == 0 ? MathHelper.PiOver4 : (MathHelper.TwoPi - MathHelper.PiOver4)) * (angle.X < 0 ? -1f : 1f);
                    Vector2 spawnPos = Projectile.Center + new Vector2(-angle.X.DirectionalSign(), 68f).RotatedBy(rotation) * Projectile.scale - Main.screenPosition;
                    Main.EntitySpriteDraw(swoosh, spawnPos, null, Color.Lerp(Color3, Color2, CurrentChargeMult) with { A = 0 } * SwingCompletion * (CurrentChargeMult * 0.75f), rotation + rotationOffset, swoosh.Size() * 0.5f, Projectile.scale * 0.45f, SpriteEffects.None);
                }
                for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.25f)
                {
                    Main.spriteBatch.Draw(
                        tex,
                        Projectile.Center + new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX.RotatedBy(i + Projectile.rotation) * outlineWidth * Projectile.scale - Main.screenPosition,
                        null,
                        MogModUtils.MulticolorLerp(CurrentChargeMult, colorList),
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