using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    // code lifted from calamity mod holy collider
    public class AstralCataclysmHoldout : BaseCustomUseStyleProjectile, ILocalizedModType
    {
        public override int AssignedItemID => ModContent.ItemType<AstralCataclysm>();
        public override LocalizedText DisplayName => MiscUtils.GetItemName<AstralCataclysm>();
        public override string Texture => "MogMod/Items/Weapons/Melee/AstralCataclysm";
        public int size = 114;
        public override float HitboxOutset => size * 0.85f;
        public override Vector2 HitboxSize => new(size, size);
        public override Vector2 SpriteOrigin => new(0, size);
        public override float HitboxRotationOffset => MathHelper.ToRadians(-45);
        public override int DamageHitCap => 15;
        public override float AdditionalScale => 0.7f;
        public Vector2 mousePos;
        public Vector2 aimVel;
        public bool doSwing = false;
        public bool postSwing = false;
        public float fadeIn = 0; // Used to make particle effects and outer glow on the sword fade in and out
        public int useAnim; // Used as your use time stat since checking the held item use time gets jank if your attack speed changes mid swing
        public int storedUseAnim; // Used to check your use time when you began using the item and to reset use time when needed
        public int swingCount = -1; // Runs counting code first, so it has to be one below

        public bool chargedSwing = false; // True if you have a charged swing fully charged
        public int chargeTimer = 0; // Timer for charging the blade
        public int chargeTimerMax = 240; // This is set to be based on use time on spawn

        public Color mainColor1 = new(255, 249, 59);
        public Color mainColor2 = new(247, 119, 224);
        public Color mainColor3 = new(40, 105, 240);
        public bool playSwingSound = true;

        public SlotId AudSlot;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void WhenSpawned()
        {
            Projectile.knockBack = 0;
            Projectile.ai[1] = -1;

            mousePos = Owner.MogMod().mouseWorld;
            aimVel = (Owner.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
            useAnim = Owner.itemAnimationMax;
            storedUseAnim = useAnim;

            chargeTimerMax = (int)(useAnim * 1.1f); // Max charge time is set here

            if (mousePos.X < Owner.Center.X) Owner.direction = -1;
            else Owner.direction = 1;

            FlipAsSword = Owner.direction == -1;
        }
        public override void UseStyle()
        {
            AnimationProgress = Animation % (chargedSwing ? (int)(storedUseAnim * 1.2f) : storedUseAnim);

            DrawUnconditionally = false;
            bool cantUse = (Owner == null || !Owner.active || Owner.dead || Main.mouseLeftRelease || Owner.CCed || Owner.noItems);

            if (CanHit || postSwing) mousePos = Owner.Center - aimVel;
            else mousePos = Owner.MogMod().mouseWorld;

            if (CanHit) fadeIn = MathHelper.Lerp(fadeIn, 1, 0.1f);
            else fadeIn = MathHelper.Lerp(fadeIn, 0, 0.15f);
            if (chargeTimer > 0) fadeIn = Utils.Remap(chargeTimer, 0, chargeTimerMax, 0, 1f);

            // If you are no longer holding the charge, then stop charge counter so you can swing
            if (cantUse)
            {
                chargeTimer = 0;
                if (Projectile.ai[2] == 5)
                {
                    Owner.itemAnimation = Owner.itemAnimationMax;
                    Projectile.timeLeft = Owner.itemAnimation;
                }
                Projectile.ai[2] = 0;
            }
            else Projectile.ai[2] = 5;

            if (!doSwing)
            {
                mousePos = Owner.MogMod().mouseWorld;
                aimVel = (Owner.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                CanHit = false;
                if (mousePos.X < Owner.Center.X) Owner.direction = -1;
                else Owner.direction = 1;
                FlipAsSword = Owner.direction == -1;

                Vector2 bladePos = new(15 * Projectile.scale, 0);
                Vector2 dustSpawnPos = Owner.Center + (bladePos).RotatedBy(FinalRotation + MathHelper.ToRadians(-45) - 0.0f * (FlipAsSword ? 1 : -1) * -Projectile.ai[1]);

                if (Projectile.ai[2] == 5)
                {
                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(120f * Projectile.ai[1] * Owner.direction), 0.05f);

                    float rotationValue = 45f + (25 * Utils.GetLerpValue(0, chargeTimerMax, chargeTimer, true)) * (FlipAsSword ? 1 : -1) * -Projectile.ai[1];
                    Projectile.rotation = Projectile.rotation.AngleLerp(Owner.AngleTo(mousePos) + MathHelper.ToRadians(rotationValue), 0.3f);
                    Animation = 0;
                    Owner.itemAnimation++;
                    Projectile.timeLeft++;

                    if (chargeTimer < chargeTimerMax && !chargedSwing)
                        chargeTimer++;

                    Vector2 dustVelocity = (Owner.Center - dustSpawnPos).SafeNormalize(Vector2.UnitX) * -9 * Projectile.scale;

                    Dust dust2 = Dust.NewDustPerfect(dustSpawnPos, DustID.HallowedTorch, dustVelocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.5f, 1.4f));
                    dust2.scale = Main.rand.NextFloat(1.45f, 1.95f) * fadeIn * Projectile.scale;
                    dust2.noGravity = true;
                    dust2.color = Main.rand.NextBool(3) ? mainColor2 : mainColor1;
                    dust2.fadeIn = Projectile.scale * 1.5f;

                    Dust dust3 = Dust.NewDustPerfect(dustSpawnPos, DustID.FireworksRGB, dustVelocity.RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 0.6f), 100, Main.rand.NextBool(3) ? mainColor2 : mainColor1, Main.rand.NextFloat(0.5f, 0.8f) * fadeIn * Projectile.scale);

                    if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                    {
                        ChargeSound.Position = Projectile.Center;
                        ChargeSound.Pitch = Utils.Remap(chargeTimer, 0, chargeTimerMax, -0.4f, 0f);
                        ChargeSound.Volume = Utils.Remap(chargeTimer, 0, chargeTimerMax, 0f, 0.5f) * 100;
                    }
                    else if (!chargedSwing) AudSlot = SoundEngine.PlaySound(SoundID.DD2_EtherianPortalIdleLoop with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
                }
                if (chargeTimer == chargeTimerMax)
                {
                    dustSpawnPos = Owner.Center + (bladePos).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));

                    SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot with { Volume = 0.7f, Pitch = 0.5f }, Projectile.Center);

                    chargedSwing = true;
                    useAnim = storedUseAnim / 2;
                    chargeTimer++;

                    for (int i = 0; i < 20; i++)
                    {
                        Dust dust4 = Dust.NewDustPerfect(dustSpawnPos, DustID.FireworksRGB, new Vector2(8, 8).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1f), 100, Main.rand.NextBool(3) ? mainColor2 : mainColor1, Main.rand.NextFloat(0.3f, 0.8f) * Projectile.scale);
                    }
                }

                if (chargeTimer == 0)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                        Projectile.localNPCImmunity[i] = 0;

                    Projectile.numHits = 0;
                    doSwing = true;
                }
            }
            else if (chargeTimer == 0)
            {
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();

                if (!CanHit && !postSwing)
                {
                    if (mousePos.X < Owner.Center.X) Owner.direction = -1;
                    else Owner.direction = 1;
                }
                else
                {
                    if ((Owner.Center - aimVel).X < Owner.Center.X) Owner.direction = -1;
                    else Owner.direction = 1;
                }

                Projectile.rotation = Projectile.rotation.AngleLerp(Owner.AngleTo(mousePos) + MathHelper.ToRadians(45f), 0.1f);

                if (AnimationProgress < (useAnim / 1.6f))
                {
                    if (Projectile.ai[2] == 5 && !chargedSwing) doSwing = false;

                    playSwingSound = true;
                    aimVel = (Owner.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                    CanHit = false;
                    postSwing = false;
                    if (AnimationProgress == 0)
                    {
                        Animation = 0;
                        doSwing = false;
                        chargeTimer = 0;
                        chargedSwing = false;
                        swingCount++;
                        useAnim = storedUseAnim;
                        Projectile.ai[1] = -Projectile.ai[1];
                    }
                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(120f * Projectile.ai[1] * Owner.direction * (1 + (Utils.GetLerpValue(useAnim * 0.15f, useAnim * 0.35f, Animation, true)) * 0.35f)), 0.2f);
                    FlipAsSword = (Owner.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX).X > 0;
                }
                else
                {
                    float time = (AnimationProgress) - (useAnim / 3);
                    float timeMax = useAnim - (useAnim / 3);

                    if (time >= (int)(timeMax * (chargedSwing ? 0.2f : 0.4f)) && playSwingSound)
                    {
                        if (!chargedSwing)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.85f, Pitch = -0.15f }, Projectile.Center);
                        }
                        else
                        {
                            SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown with { Volume = 0.8f, Pitch = -0.35f }, Projectile.Center);
                            SoundEngine.PlaySound(SoundID.DD2_SonicBoomBladeSlash with { Volume = 0.9f, Pitch = -0.55f }, Projectile.Center);
                        }
                        playSwingSound = false;
                    }
                    if (time > (int)(timeMax * (chargedSwing ? 0.1f : 0.3f)) && time < (int)(timeMax * (chargedSwing ? 0.95f : 0.85f)))
                    {
                        CanHit = true;

                        Vector2 dustVelocity = new Vector2(0, 2 * -Projectile.ai[1] * Owner.direction).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));
                        Vector2 dustSpawnPos = Owner.Center + (new Vector2(Main.rand.Next(30, 170) * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)));
                        if (!chargedSwing)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                dustVelocity = (new Vector2(0, 15 * -Projectile.ai[1] * Owner.direction) * Main.rand.NextFloat(0.3f, 1f)).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));
                                dustSpawnPos = Owner.Center + (new Vector2(Main.rand.Next(30, 170) * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)));
                                if (i < 2)
                                {
                                    Dust dust2 = Dust.NewDustPerfect(dustSpawnPos, DustID.AncientLight, -dustVelocity.RotatedByRandom(0.3f));
                                    dust2.scale = Main.rand.NextFloat(0.95f, 1.45f) * Projectile.scale;
                                    dust2.noGravity = true;
                                    dust2.color = Main.rand.NextBool(3) ? mainColor2 : mainColor1;
                                    dust2.fadeIn = Projectile.scale - 1;
                                }
                                else
                                {
                                    Dust dust2 = Dust.NewDustPerfect(dustSpawnPos, DustID.AncientLight, (-dustVelocity * 0.2f).RotatedByRandom(0.3f), 100, Color.Lerp(Color.Orchid, Color.White, Main.rand.NextFloat(0, 0.7f)), Main.rand.NextFloat(0.9f, 1.1f) * Projectile.scale);
                                    dust2.fadeIn = Projectile.scale - 1;
                                }
                            }
                        }
                    }
                    else CanHit = false;

                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(MathHelper.Lerp(150f * Projectile.ai[1] * Owner.direction, 120f * -Projectile.ai[1] * Owner.direction, MiscUtils.ExpInOutEasing(time / timeMax, 1))), 0.2f);

                    if (time < (int)(timeMax * 0.9f)) postSwing = true;

                    if (CanHit)
                    {
                        if (chargedSwing)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                float randRot = Main.rand.NextFloat(-10, -45);
                                Vector2 dustVel = (new Vector2(0, 15 * -Projectile.ai[1] * Owner.direction)).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));
                                Vector2 spawnPos = Owner.Center + new Vector2(170 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot)).RotatedByRandom(0.4f);
                                Dust dust2 = Dust.NewDustPerfect(spawnPos, DustID.AncientLight, -dustVel * Main.rand.NextFloat(0.4f, 0.7f), 100, Main.rand.NextBool(4) ? Color.DarkGoldenrod : Color.Goldenrod, Main.rand.NextFloat(0.3f, 0.35f) * Projectile.scale);
                            }
                            for (int i = 0; i < 6; i++)
                            {
                                float randRot = Main.rand.NextFloat(-30, -60);
                                Vector2 dustVel = (new Vector2(0, 35 * -Projectile.ai[1] * Owner.direction)).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));
                                Dust dust2 = Dust.NewDustPerfect(Owner.Center + (new Vector2(170 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f)), DustID.FireworksRGB, dustVel * Main.rand.NextFloat(0.2f, 0.6f));
                                dust2.scale = Main.rand.NextFloat(1.35f, 1.85f) * Projectile.scale;
                                dust2.noGravity = true;
                                dust2.color = Main.rand.NextBool(3) ? mainColor2 : mainColor1;
                                dust2.fadeIn = Projectile.scale * 0.3f;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                float randRot = Main.rand.NextFloat(-30, -60);
                                Vector2 dustVel = -(new Vector2(0, 25 * -Projectile.ai[1] * Owner.direction)).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));
                                Dust dust2 = Dust.NewDustPerfect(Owner.Center + (new Vector2(170 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.15f)), DustID.FireworksRGB, dustVel * Main.rand.NextFloat(0.1f, 0.5f));
                                dust2.scale = Main.rand.NextFloat(0.75f, 0.9f) * Projectile.scale;
                                dust2.noGravity = true;
                                dust2.color = Main.rand.NextBool(3) ? mainColor2 : mainColor1;
                                dust2.fadeIn = Projectile.scale - 1;
                            }
                        }
                    }
                }
            }
            ArmRotationOffset = MathHelper.ToRadians(-140f);
            ArmRotationOffsetBack = MathHelper.ToRadians(-140f);
        }
        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound))
                ChargeSound?.Stop();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BlackBladeDebuff>(), 300);

            if (!chargedSwing)
            {
                if (Projectile.numHits == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce with { Volume = 0.85f, PitchVariance = 0.25f }, Projectile.Center);
                }
            }
            else
            {
                if (Projectile.numHits == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost with { Volume = 1f, PitchVariance = 0.15f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { Volume = 0.9f, PitchVariance = 0.15f }, Projectile.Center);

                    float starAngle = MathHelper.ToRadians(45f);
                    for (int i = 0; i < 4; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB);
                        Vector2 vel = (MathHelper.TwoPi * i / 4f).ToRotationVector2().RotatedBy(starAngle) * 4f;
                        Dust dust2 = Dust.NewDustPerfect(target.Center, DustID.FireworksRGB, vel, 80, Color.Red, 1.2f * Projectile.scale);
                    }
                }
            }

            Vector2 launchVel = Utils.DirectionTo(Owner.Center, Owner.MogMod().mouseWorld);
            target.MoveNPC(launchVel, (chargedSwing ? 35 : 23), true, Owner);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.SourceDamage *= (chargedSwing ? 1f : 0.2f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // Only draw the projectile if the projectile's owner is currently using the item this projectile is attached to.
            if ((useAnim > 0 || DrawUnconditionally) && (Owner.ItemAnimationActive))
            {
                Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
                bool flipAsSword = (swingCount % 2 == 0 ? !FlipAsSword : FlipAsSword);
                float r = flipAsSword ? MathHelper.ToRadians(90) : 0f;
                Vector2 generalDrawPos = Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY);
                SpriteEffects sEffects = spriteEffects != SpriteEffects.None ? spriteEffects : (flipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

                for (int i = 0; i < 25; i++)
                {
                    Texture2D centerTexture = ModContent.Request<Texture2D>("MogMod/Projectiles/Melee/AstralCataclysmGhost").Value;
                    Color auraColor = mainColor1 with { A = 0 } * 0.15f * fadeIn;
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 25f).ToRotationVector2() * 6 * fadeIn;
                    Main.EntitySpriteDraw(centerTexture, Projectile.Center - Main.screenPosition + drawOffset + new Vector2(0, Owner.gfxOffY), centerTexture.Frame(1, FrameCount, 0, Frame), auraColor, Projectile.rotation + RotationOffset + r, flipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (flipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                }
                Main.EntitySpriteDraw(tex.Value, generalDrawPos, tex.Frame(1, FrameCount, 0, Frame), lightColor, Projectile.rotation + RotationOffset + r, flipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, sEffects);
            }
            else
            {
                chargeTimer = 0;
                chargedSwing = false;
            }
            return false;
        }
        public override void ResetStyle()
        {
        }
    }
}