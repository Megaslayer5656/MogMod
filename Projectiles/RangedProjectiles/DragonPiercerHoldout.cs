using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Config;
using MogMod.Items.Weapons.Ranged;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class DragonPiercerHoldout : BaseGunHoldoutProjectile
    {
        public static readonly SoundStyle WeakCharge = new($"{nameof(MogMod)}/Sounds/SE/bowChargeWeak") { Volume = 1.1f, PitchVariance = .2f, MaxInstances = 5 };
        public static readonly SoundStyle StrongCharge = new($"{nameof(MogMod)}/Sounds/SE/bowChargeStrong") { Volume = 1.1f, PitchVariance = .2f, MaxInstances = 2 };
        public override int AssociatedItemID => ModContent.ItemType<DragonPiercer>();
        private Asset<Texture2D> ItemTexture => TextureAssets.Item[AssociatedItemID];
        public override float RecoilResolveSpeed => (ChargeLvl3 && !HoldingRightClick ? 0.04f : ChargeLvl2 || HoldingRightClick ? 0.12f : ChargeLvl1 ? 0.2f : 0.4f);
        public override Vector2 GunTipPosition => Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.5f;
        public override float MaxOffsetLengthFromArm => 22f;
        public ref float Time => ref Projectile.ai[0];
        public ref float DrawTimer => ref Projectile.ai[1];
        public ref float DecayCounter => ref Projectile.ai[2];
        public float Cap = 10f;
        public float Spread = 0.1f;
        public int MaxShots = DragonPiercer.MaxShots;
        public int MinCharge = DragonPiercer.MinCharge;
        public int MaxCharge = DragonPiercer.MaxCharge;
        public bool ChargeLvl1 = false;
        public bool ChargeLvl2 = false;
        public bool ChargeLvl3 = false;
        public bool StartedChargeLvl1 = false;
        public bool StartedChargeLvl2 = false;
        public bool StartedChargeLvl3 = false;
        public bool FiredProj = false;
        public int NewMinCharge = DragonPiercer.MinCharge;
        public int NewMaxCharge = DragonPiercer.MaxCharge;
        public bool HoldingRightClick = false;
        public override void SetDefaults()
        {
            Projectile.width = ItemTexture.Width();
            Projectile.height = ItemTexture.Height();
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }
        public override void KillHoldoutLogic()
        {
            if (DecayCounter <= 0 && (Owner.CantUseHoldout() || HeldItem.type != AssociatedItemID)) Projectile.Kill();
            if (!Owner.active || Owner.dead) Projectile.Kill();
        }
        public override void HoldoutAI()
        {
            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 20f;
            // get the max charge adjusted for attack speed
            var attackSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
            if (attackSpeed > Cap) attackSpeed = Cap;
            if (attackSpeed != 0f) attackSpeed = 1f / attackSpeed;
            NewMinCharge = (int)(MinCharge * attackSpeed);
            NewMaxCharge = (int)(NewMaxCharge * attackSpeed);
            ChargeLvl1 = Time >= (int)((MaxCharge - 120) * attackSpeed);
            ChargeLvl2 = Time >= (int)((MaxCharge - 60) * attackSpeed);
            ChargeLvl3 = Time >= (NewMaxCharge = (int)(MaxCharge * attackSpeed));

            Owner.channel = Main.mouseLeft;
            // fire proj if not holding
            if (FiredProj && Owner.channel)
            {
                Time = 0;
                DrawTimer = 0;
                ChargeLvl1 = ChargeLvl2 = ChargeLvl3 = StartedChargeLvl1 = StartedChargeLvl2 = StartedChargeLvl3 = false;
                HoldingRightClick = Owner.MogMod().mouseRight;
                FiredProj = false;
            }
            if (Owner.CantUseHoldout())
            {
                // only fire if there are charges
                if (ChargeLvl1 && !FiredProj)
                {
                    Shoot();
                    FiredProj = true;
                }
                DecayCounter--;
            }
            else if (Owner.MogMod().mouseRight)
            {
                if (!ChargeLvl1)
                {
                    StartedChargeLvl1 = StartedChargeLvl2 = StartedChargeLvl3 = false;
                    HoldingRightClick = true;
                }
                // once reaching lvl 1 charge
                if (ChargeLvl1)
                {
                    // if any charges were present before right clicking, reset variables
                    if (!HoldingRightClick)
                    {
                        Time = 0;
                        DrawTimer = 0;
                        DecayCounter = 0;
                        ChargeLvl1 = ChargeLvl2 = ChargeLvl3 = false;
                        StartedChargeLvl1 = StartedChargeLvl2 = StartedChargeLvl3 = true;
                        HoldingRightClick = true;
                        FiredProj = false;
                    }
                    if (!StartedChargeLvl1)
                    {
                        SoundEngine.PlaySound(WeakCharge with { Pitch = -0.15f }, Projectile.Center);
                        for (int i = 0; i < 75; i++)
                        {
                            float colorRando = Main.rand.NextFloat(0, 1);
                            float offsetAngle = MathHelper.TwoPi * i / 75f;
                            float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 3D);
                            float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 3D);

                            Vector2 vel = new Vector2(unitOffsetX, unitOffsetY) * 2.5f;
                            Dust charged = Dust.NewDustPerfect(GunTipPosition, 267, vel);
                            charged.scale = 0.75f;
                            charged.fadeIn = 0.5f;
                            charged.color = Color.Lerp(Color.Yellow, Color.Gold, colorRando);
                            charged.noGravity = true;
                        }
                        StartedChargeLvl1 = true;
                        DecayCounter = NewMinCharge;
                    }
                    int dustSpot = 6;
                    for (int i = 0; i <= 5; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(GunTipPosition - Projectile.velocity * dustSpot, Main.rand.NextBool(3) ? DustID.Flare : DustID.Torch, -shootVelocity.RotatedByRandom(MathHelper.ToRadians(10f)) * Main.rand.NextFloat(0.2f, 1.2f), 0, default, Main.rand.NextFloat(1f, 2.3f));
                        dust.scale = 1f;
                        dust.alpha = 100;
                        dust.noGravity = true;
                    }
                }
                if (!ChargeLvl1 && Time >= NewMinCharge) DrawTimer++;
                if (!ChargeLvl1) Time++;
            }
            else
            {
                if (!ChargeLvl1)
                {
                    StartedChargeLvl1 = StartedChargeLvl2 = StartedChargeLvl3 = false;
                    HoldingRightClick = false;
                }
                if (ChargeLvl1)
                {
                    // reset right click variable
                    if (HoldingRightClick)
                    {
                        Time = 0;
                        DrawTimer = 0;
                        DecayCounter = 0;
                        ChargeLvl1 = ChargeLvl2 = ChargeLvl3 = false;
                        StartedChargeLvl1 = StartedChargeLvl2 = StartedChargeLvl3 = true;
                        HoldingRightClick = false;
                        FiredProj = false;
                    }
                    if (!StartedChargeLvl1)
                    {
                        SoundEngine.PlaySound(WeakCharge with { Volume = 0.9f, Pitch = 0.1f }, Projectile.Center);
                        for (int i = 0; i < 75; i++)
                        {
                            float colorRando = Main.rand.NextFloat(0, 1);
                            float offsetAngle = MathHelper.TwoPi * i / 75f;
                            float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 3D);
                            float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 3D);

                            Vector2 vel = new Vector2(unitOffsetX, unitOffsetY) * 2.5f;
                            Dust charged = Dust.NewDustPerfect(GunTipPosition, 267, vel);
                            charged.scale = 0.75f;
                            charged.fadeIn = 0.5f;
                            charged.color = Color.Lerp(Color.Yellow, Color.Gold, colorRando);
                            charged.noGravity = true;
                        }
                        DrawTimer = 0;
                        StartedChargeLvl1 = true;
                        DecayCounter = NewMinCharge;
                    }
                    int dustSpot = 6;
                    for (int i = 0; i <= 5; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(GunTipPosition - Projectile.velocity * dustSpot, DustID.Torch, -shootVelocity.RotatedByRandom(MathHelper.ToRadians(5f)) * Main.rand.NextFloat(0.2f, 1.2f), 0, default, Main.rand.NextFloat(1f, 2.3f));
                        dust.scale = 0.75f;
                        dust.alpha = 100;
                        dust.noGravity = true;
                    }
                    if (ChargeLvl2)
                    {
                        if (!StartedChargeLvl2)
                        {
                            SoundEngine.PlaySound(WeakCharge, Projectile.Center);
                            for (int i = 0; i < 75; i++)
                            {
                                float colorRando = Main.rand.NextFloat(0, 1);
                                float offsetAngle = MathHelper.TwoPi * i / 75f;
                                float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 3D);
                                float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 3D);

                                Vector2 vel = new Vector2(unitOffsetX, unitOffsetY) * 2.5f;
                                Dust charged = Dust.NewDustPerfect(GunTipPosition, 267, vel);
                                charged.scale = 0.75f;
                                charged.fadeIn = 0.5f;
                                charged.color = Color.Lerp(Color.Yellow, Color.Gold, colorRando);
                                charged.noGravity = true;
                            }
                            DrawTimer = 0;
                            StartedChargeLvl2 = true;
                        }
                        for (int i = 0; i <= 5; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(GunTipPosition - Projectile.velocity * dustSpot, DustID.Flare, -shootVelocity.RotatedByRandom(MathHelper.ToRadians(13f)) * Main.rand.NextFloat(0.2f, 1.2f), 0, default, Main.rand.NextFloat(1f, 2.3f));
                            dust.scale = 1.5f;
                            dust.alpha = 100;
                            dust.noGravity = true;
                        }
                        if (ChargeLvl3)
                        {
                            if (!StartedChargeLvl3)
                            {
                                SoundEngine.PlaySound(StrongCharge, Projectile.Center);
                                for (int i = 0; i < 75; i++)
                                {
                                    float colorRando = Main.rand.NextFloat(0, 1);
                                    float offsetAngle = MathHelper.TwoPi * i / 75f;

                                    float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 3D);
                                    float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 3D);

                                    Vector2 vel = new Vector2(unitOffsetX, unitOffsetY) * 5f;
                                    Dust charged = Dust.NewDustPerfect(GunTipPosition, 267, vel);
                                    charged.scale = 1.5f;
                                    charged.fadeIn = 0.5f;
                                    charged.color = Color.Lerp(Color.PaleVioletRed, Color.MediumVioletRed, colorRando);
                                    charged.noGravity = true;
                                }
                                StartedChargeLvl3 = true;
                            }
                            for (int i = 0; i <= 5; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(GunTipPosition - Projectile.velocity * dustSpot, DustID.Flare, -shootVelocity.RotatedByRandom(MathHelper.ToRadians(18f)) * Main.rand.NextFloat(0.2f, 1.2f), 0, default, Main.rand.NextFloat(1f, 2.3f));
                                dust.scale = 2f;
                                dust.alpha = 100;
                                dust.noGravity = true;
                            }
                        }
                    }
                }

                if (!ChargeLvl3 && Time >= NewMinCharge) DrawTimer++;
                if (!ChargeLvl3) Time++;
            }
        }
        public void Shoot()
        {
            if (Owner.MogMod().mouseRight)
            {
                Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 20f;
                if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm += 5f; // visual recoil effect
                Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int damage, out float knockback, out _);
                var source = Projectile.GetSource_FromThis();
                int type = ModContent.ProjectileType<TracerArrow>();
                Vector2 shootPos = Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.25f;
                SoundEngine.PlaySound(SoundID.Item102, Projectile.Center);
                foreach (Projectile proj in Main.projectile) if (proj.type == type) proj.Kill();
                Projectile.NewProjectile(source, shootPos, shootVelocity, type, damage, knockback, Projectile.owner);
            }
            else
            {
                Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 20f;
                SoundEngine.PlaySound(SoundID.Item5 with { Volume = 0.3f, Pitch = 0.05f, PitchVariance = 0.1f, MaxInstances = -1 }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item17 with { Volume = 0.7f, Pitch = -0.05f, PitchVariance = 0.1f, MaxInstances = -1 }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item23 with { Volume = 0.45f, Pitch = 0.15f, PitchVariance = 0.1f, MaxInstances = -1 }, Projectile.Center);
                // fire dusts
                Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, Vector2.Zero, 100, Color.DarkGoldenrod, Main.rand.NextFloat(0.8f, 1.2f));
                for (int i = 0; i <= 12; i++)
                {
                    Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, (shootVelocity * Main.rand.NextFloat(0.2f, 1.1f)).RotatedByRandom(0.4f), 0, default);
                    dust2.noGravity = true;
                    dust2.scale = Main.rand.NextFloat(0.8f, 1.4f);
                }
                Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int damage, out float knockback, out _);
                if (Main.myPlayer == Projectile.owner)
                {
                    var source = Projectile.GetSource_FromThis();
                    int type = ModContent.ProjectileType<DragonPiercerArrow>();
                    Vector2 shootPos = Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.25f;
                    if (ChargeLvl3) damage = (int)(damage * DragonPiercer.DamageMult);
                    if (ChargeLvl1)
                    {
                        Projectile.NewProjectile(source, shootPos, shootVelocity, type, damage, knockback, Projectile.owner);
                        if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm += 3f;
                        SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot, Owner.Center);
                        if (ChargeLvl2)
                        {
                            Projectile.NewProjectile(source, shootPos, shootVelocity.RotatedBy(Spread), type, damage, knockback, Projectile.owner);
                            Projectile.NewProjectile(source, shootPos, shootVelocity.RotatedBy(-Spread), type, damage, knockback, Projectile.owner);
                            if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm += 3f;
                            if (ChargeLvl3)
                            {
                                Projectile.NewProjectile(source, shootPos, shootVelocity.RotatedBy(Spread * 2f), type, damage, knockback, Projectile.owner);
                                Projectile.NewProjectile(source, shootPos, shootVelocity.RotatedBy(-Spread * 2f), type, damage, knockback, Projectile.owner);
                                if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm += 4f;
                                SoundEngine.PlaySound(SoundID.Item102 with { Pitch = -0.2f }, Projectile.Center);
                            }
                        }
                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f) - (Owner.gravDir == -1 ? MathHelper.Pi * Owner.direction : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Color auraColor = Color.White;

            float opacity = Utils.GetLerpValue(0, NewMaxCharge / 3, DrawTimer / 3, true);
            float bolts = HoldingRightClick ? 1 : ChargeLvl2 ? 5 : ChargeLvl1 ? 3 : 1;
            for (int i = 0; i < bolts; i++)
            {
                float BoltAngle;
                if (bolts == 1) BoltAngle = 0;
                else if (bolts == 3)
                {
                    float increment = Spread;
                    BoltAngle = increment * (i - 1);
                }
                else
                {
                    float increment = Spread * 2;
                    BoltAngle = increment * (i - 2);
                }
                Color Transparency = Projectile.GetAlpha(lightColor) * (opacity * 2.2f);
                var BoltTexture = ModContent.Request<Texture2D>("MogMod/Projectiles/RangedProjectiles/DragonPiercerArrow").Value;
                if (Owner.MogMod().mouseRight) BoltTexture = ModContent.Request<Texture2D>("MogMod/Projectiles/RangedProjectiles/TracerArrow").Value;
                Vector2 PointingTo = new((float)Math.Cos(Projectile.rotation + BoltAngle), (float)Math.Sin(Projectile.rotation + BoltAngle));
                Vector2 ShiftDown = PointingTo.RotatedBy(-MathHelper.PiOver2);
                float FlipFactor = Owner.direction < 0 ? MathHelper.Pi : 0f;
                Vector2 boltPos = Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.25f - Main.screenPosition;
                Vector2 drawOffset = ((MathHelper.TwoPi * i / 16f).ToRotationVector2() * 5);
                if (!Owner.CantUseHoldout()) Main.EntitySpriteDraw(BoltTexture, boltPos + drawOffset, null, Transparency, drawRotation + (BoltAngle * 1f) + MathHelper.PiOver2 + FlipFactor, BoltTexture.Size() * 0.5f, 1f, flipSprite, 0);
            }
            auraColor = (HoldingRightClick ? new(255, 233, 186) : ChargeLvl2 ? new(255, 25, 75) : ChargeLvl1 ? Color.Goldenrod : Color.PaleGoldenrod) * opacity * 0.8f;
            if (HoldingRightClick && ChargeLvl1 || ChargeLvl3)
            {
                if (MogClientConfig.Instance.GunRecoil)
                {
                    float rumble = MathHelper.Clamp(DrawTimer / (HoldingRightClick ? 2f : 1f), 0f, NewMaxCharge);
                    drawPosition += Main.rand.NextVector2Circular(rumble / 70f, rumble / 70f);
                }
            }
            if (Time >= NewMinCharge)
            {
                for (int i = 0; i < 16; i++)
                {
                    Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/DragonPiercerGhost").Value;
                    Vector2 drawOffset = ((MathHelper.TwoPi * i / 16f).ToRotationVector2() * 5);
                    Main.EntitySpriteDraw(ghost, drawPosition + drawOffset, null, auraColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);
                }
            }
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale, flipSprite);
            return false;
        }
    }
}