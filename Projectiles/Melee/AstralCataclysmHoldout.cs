using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    // TODO: add charging sound and hit check for custom star projectiles
    public class AstralCataclysmHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public Player Owner => Main.player[Projectile.owner];
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<AstralCataclysm>()).Item;
        public override string Texture => BaseItem.ModItem.Texture;
        public override int AfterImageLength => 10;
        public override int OffsetDistance => 80;
        public override int CooldownTime { get; set; }
        public override SoundStyle? UseSound => SoundID.DD2_MonkStaffSwing with { Volume = 1f };
        public ref float CurrentChargeMult => ref Projectile.ai[0];
        public ref float DustTimer => ref Projectile.ai[1];
        bool playedChargeSound = false;
        bool justRightClicked = false;
        Color Color1 = AstralCataclysm.MainColor1;
        Color Color2 = AstralCataclysm.MainColor2;
        Color Color3 = AstralCataclysm.MainColor3;
        public static readonly Color[] colorList =
        [
            AstralCataclysm.MainColor1,
            AstralCataclysm.MainColor2,
            AstralCataclysm.MainColor3
        ];
        public override void Defaults()
        {
            Projectile.extraUpdates = 3;
            swingWidth = 200;
            RotateInCooldown = 0.3f;
            RotateInStartup = 0.3f;
        }
        public override void Spawn()
        {
            StartupTime = Main.zenithWorld ? 360 : 80;
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
                Color color1 = Main.rand.NextBool(4) ? Color2 : Color1;
                Color color2 = Main.rand.NextBool(4) ? Color1 : Color2;
                if (Owner.MogMod().mouseRight)
                {
                    color1 = Main.rand.NextBool(4) ? Color2 : Color3;
                    color2 = Main.rand.NextBool(4) ? Color3 : Color2;
                    if (!justRightClicked)
                    {
                        CurrentChargeMult = 0f;
                        DustTimer = 0f;
                        justRightClicked = true;
                    }
                }
                else if (justRightClicked)
                {
                    CurrentChargeMult = 0f;
                    DustTimer = 0f;
                    justRightClicked = false;
                }
                if (DustTimer % Projectile.extraUpdates == 0)
                {
                    Vector2 dustVel = new Vector2(-60 * -Projectile.spriteDirection, -5).RotatedBy(Projectile.rotation + 0.7f * -Projectile.spriteDirection);
                    Vector2 spawnPos = Projectile.Center - dustVel.RotatedByRandom(0.4f) * Projectile.scale * CurrentChargeMult;

                    Dust dust2 = Dust.NewDustPerfect(spawnPos, DustID.RainbowTorch, dustVel.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.5f, 1.4f) * 0.3f);
                    dust2.scale = Main.rand.NextFloat(1.45f, 1.95f) * CurrentChargeMult * Projectile.scale;
                    dust2.noGravity = true;
                    dust2.color = color1;
                    dust2.fadeIn = CurrentChargeMult;

                    Dust dust3 = Dust.NewDustPerfect(spawnPos, DustID.FireworksRGB, dustVel.RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 0.6f) * 0.3f, 100, color2, Main.rand.NextFloat(0.5f, 0.8f) * CurrentChargeMult * Projectile.scale);
                }
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
                        SoundEngine.PlaySound(SoundID.DeerclopsStep with { Volume = 2f, Pitch = 0.5f }, Projectile.Center);
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
                var type = ModContent.ProjectileType<AstralStar>();
                Vector2 aimVel(float velocity) => (Owner.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * velocity;
                if (justRightClicked)
                {
                    foreach (Projectile star in Main.ActiveProjectiles)
                    {
                        //Vector2 dustVel = veloc.RotatedBy(MathHelper.PiOver4 * 0.5f * Projectile.spriteDirection) * Main.rand.NextFloat(2, 5);
                        //Vector2 bladePos = Projectile.Center + new Vector2(-angle.X.DirectionalSign(), Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation - 0.7f * Projectile.spriteDirection) * ( * (mogPlayer.swingNum % 2 == 0 ? -1f : 1f)) * Projectile.scale;
                        if (Vector2.Distance(Projectile.Center, star.Center) <= 90 * Projectile.scale + Math.Max(star.width, star.height) && star.active)
                        {
                            if (star.type == type)
                            {
                                if (CurrentChargeMult >= 1)
                                {
                                    star.velocity += aimVel(2.5f);
                                    star.timeLeft = 600;
                                    star.numHits++;
                                    star.owner = Owner.whoAmI;
                                    star.netUpdate = true;
                                }
                                else
                                {
                                    star.velocity += aimVel(1f);
                                    star.timeLeft = 300;
                                    star.numHits++;
                                    star.owner = Owner.whoAmI;
                                    star.netUpdate = true;
                                }
                            }
                        }
                    }
                }
                else if (timer % swingTime / 3 == 0)
                {
                    for (float i = -0.25f; i < 0.26f; i += 0.50f)
                    {
                        if (Projectile.owner == Main.myPlayer)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, -(aimVel(70f * CurrentChargeMult) / 4).RotatedByRandom(i) * Main.rand.NextFloat(0.6f, 1.4f), type, Projectile.damage, Projectile.knockBack, Projectile.owner);
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
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (CurrentChargeMult >= 1)
            {
                if (Projectile.numHits == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost with { Volume = 1f, PitchVariance = 0.15f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { Volume = 0.9f, PitchVariance = 0.15f }, Projectile.Center);

                    float starAngle = MathHelper.ToRadians(45f);
                    for (int i = 0; i < 4; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, newColor: Main.rand.NextBool() ? Color1 : Color2);
                        Vector2 vel = (MathHelper.TwoPi * i / 4f).ToRotationVector2().RotatedBy(starAngle) * 4f;
                        Dust dust2 = Dust.NewDustPerfect(target.Center, DustID.FireworksRGB, vel, 80, Color2, 1.2f * Projectile.scale);
                    }
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
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!inCooldown)
            {
                var tex = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/AstralCataclysmGhost").Value;
                float outlineWidth = (int)(4 * CurrentChargeMult) * 0.5f;
                if (inSwing) outlineWidth *= 1 - SwingCompletion;
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