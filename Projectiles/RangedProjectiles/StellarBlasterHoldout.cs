using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Config;
using MogMod.Items.Weapons.Ranged;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class StellarBlasterHoldout : BaseGunHoldoutProjectile
    {
        public static readonly SoundStyle WeakCharge = new($"{nameof(MogMod)}/Sounds/SE/bowChargeWeak") { Volume = 1.1f, PitchVariance = .2f, MaxInstances = 5 };
        public static readonly SoundStyle StrongCharge = new($"{nameof(MogMod)}/Sounds/SE/bowChargeStrong") { Volume = 1.1f, PitchVariance = .2f, MaxInstances = 2 };
        public override int AssociatedItemID => ModContent.ItemType<StellarBlaster>();
        public override Vector2 GunTipPosition => Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.5f;
        public override float MaxOffsetLengthFromArm => 20f;
        public override float RecoilResolveSpeed => 0.1f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -5f;
        public override float OffsetYDownwards => 5f;
        public ref float Timer => ref Projectile.ai[0];
        public ref float DrawTimer => ref Projectile.ai[1];
        public ref float DecayCounter => ref Projectile.ai[2];
        public float Cap = 20f;
        public int MinCharge = StellarBlaster.MinCharge;
        public int MaxCharge = StellarBlaster.MaxCharge;
        public int NewMinCharge = StellarBlaster.MinCharge;
        public int NewMaxCharge = StellarBlaster.MaxCharge;
        public bool ChargeLvl1 = false;
        public bool ChargeLvl2 = false;
        public bool ChargeLvl3 = false;
        public bool StartedChargeLvl1 = false;
        public bool StartedChargeLvl2 = false;
        public bool StartedChargeLvl3 = false;
        public bool FiredProj = false;
        public bool HoldingRightClick = false;
        public float StarCharge = 0f;
        public static readonly Color[] colorList =
        [
            StellarBlaster.MainColor1,
            StellarBlaster.MainColor2,
            StellarBlaster.MainColor3
        ];
        public override void KillHoldoutLogic()
        {
            if (DecayCounter <= 0 && (Owner.CantUseHoldout() || HeldItem.type != AssociatedItemID)) Projectile.Kill();
            if (!Owner.active || Owner.dead) Projectile.Kill();
        }
        public override void HoldoutAI()
        {
            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 10f;
            Vector2 shootPos = Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.25f;
            // get the max charge adjusted for attack speed
            var attackSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
            if (attackSpeed > Cap) attackSpeed = Cap;
            if (attackSpeed != 0f) attackSpeed = 1f / attackSpeed;
            NewMinCharge = (int)(MinCharge * attackSpeed);
            NewMaxCharge = (int)(NewMaxCharge * attackSpeed);
            ChargeLvl1 = Timer >= (int)((MaxCharge - 120) * attackSpeed);
            ChargeLvl2 = Timer >= (int)((MaxCharge - 60) * attackSpeed);
            ChargeLvl3 = Timer >= (NewMaxCharge = (int)(MaxCharge * attackSpeed));

            Owner.channel = Main.mouseLeft;
            // fire proj if not holding
            if (FiredProj && Owner.channel)
            {
                Timer = 0;
                DrawTimer = 0;
                ChargeLvl1 = ChargeLvl2 = ChargeLvl3 = StartedChargeLvl1 = StartedChargeLvl2 = StartedChargeLvl3 = false;
                HoldingRightClick = Owner.MogMod().mouseRight;
                FiredProj = false;
            }
            int type = ModContent.ProjectileType<ChargedStellarStar>();
            if (Owner.CantUseHoldout())
            {
                // only fire if there are charges
                if (ChargeLvl1 && !FiredProj)
                {
                    SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot with { Pitch = StarCharge * 0.15f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.Item92 with { Pitch = StarCharge * 0.1f}, Projectile.Center);
                    if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 5f * StarCharge; // visual recoil effect
                    foreach (Projectile star in Main.ActiveProjectiles)
                    {
                        if (star.type == type && star.owner == Main.myPlayer)
                        {
                            star.velocity = shootVelocity * (0.5f + StarCharge);
                            //star.ai[0] = StarCharge;
                        }
                    }
                    FiredProj = true;
                }
                DecayCounter--;
            }
            else if (Owner.MogMod().mouseRight)
            {
                if (!ChargeLvl1)
                {
                    StarCharge = 0f;
                    StartedChargeLvl1 = StartedChargeLvl2 = StartedChargeLvl3 = false;
                    HoldingRightClick = true;
                }
                // once reaching lvl 1 charge
                if (ChargeLvl1)
                {
                    StarCharge = Timer / NewMaxCharge;
                    Owner.PickAmmo(Owner.HeldItem, out int ammoValue, out float ammoSpeed, out int ammoDamage, out float ammoKnockback, out _, true);
                    int newDamage = (int)(ammoDamage * (StarCharge * 3f));
                    float newKnockback = (int)(ammoKnockback * (StarCharge * 2f));
                    // if any charges were present before right clicking, reset variables
                    if (!HoldingRightClick)
                    {
                        Timer = 0;
                        DrawTimer = 0;
                        DecayCounter = 0;
                        ChargeLvl1 = ChargeLvl2 = ChargeLvl3 = false;
                        StartedChargeLvl1 = StartedChargeLvl2 = StartedChargeLvl3 = true;
                        HoldingRightClick = true;
                        FiredProj = false;
                    }
                    if (!StartedChargeLvl1)
                    {
                        Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int damage, out float knockback, out _);
                        var source = Projectile.GetSource_FromThis();
                        if (Main.myPlayer == Projectile.owner)
                        {
                            foreach (Projectile proj in Main.projectile) if (proj.type == type && proj.owner == Main.myPlayer) proj.Kill();
                            Projectile star = Projectile.NewProjectileDirect(source, shootPos, Vector2.Zero, type, damage, knockback, Projectile.owner, StarCharge);
                        }

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
                        StartedChargeLvl1 = true;
                        DecayCounter = NewMinCharge * 2f;
                    }
                    foreach (Projectile star in Main.ActiveProjectiles)
                    {
                        if (star.type == type && star.owner == Main.myPlayer)
                            {
                            star.Center = shootPos;
                            star.velocity = shootVelocity;
                            star.ai[0] = StarCharge;
                            star.originalDamage = newDamage;
                            star.damage = newDamage;
                            star.knockBack = newKnockback;
                            star.timeLeft = 600;
                        }
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
                            StartedChargeLvl2 = true;
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
                        }
                    }
                }
                if (!ChargeLvl3 && ChargeLvl1 && Timer >= NewMinCharge) DrawTimer++;
                if (!ChargeLvl3) Timer++;
            }
            else
            {
                if (Timer >= (int)((MaxCharge - 126) * attackSpeed)) if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 15f * Math.Abs(1f - (Timer * 0.015f));
                if (!ChargeLvl1)
                {
                    StarCharge = 0f;
                    StartedChargeLvl1 = StartedChargeLvl2 = StartedChargeLvl3 = false;
                    HoldingRightClick = false;
                    for (int i = 0; i < 2; i++)
                    {
                        float rotMulti = Main.rand.NextFloat(0.3f, 1f);
                        float drawSpeed = MathF.Sin(Main.GlobalTimeWrappedHourly * 4) * 0.5f + 0.5f;
                        Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(5) ? DustID.FireworksRGB : DustID.RainbowTorch, newColor: Color.WhiteSmoke);
                        dust2.noGravity = true;
                        dust2.velocity = new Vector2(0, 0).RotatedByRandom(rotMulti * 0.3f) * (Main.rand.NextFloat(1f, 2.9f) - rotMulti);
                        dust2.scale = Main.rand.NextFloat(1.2f, 1.8f) * (Timer * 0.015f);
                        dust2.color = MogModUtils.MulticolorLerp(drawSpeed, colorList);
                    }
                }
                if (ChargeLvl1)
                {
                    // reset right click variable
                    if (HoldingRightClick)
                    {
                        Timer = 0;
                        DrawTimer = 0;
                        DecayCounter = 0;
                        ChargeLvl1 = ChargeLvl2 = ChargeLvl3 = false;
                        StartedChargeLvl1 = StartedChargeLvl2 = StartedChargeLvl3 = true;
                        HoldingRightClick = false;
                        FiredProj = false;
                    }
                    if (!StartedChargeLvl1)
                    {
                        SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
                        SoundEngine.PlaySound(SoundID.DD2_BookStaffCast with { Volume = 0.8f, Pitch = Main.rand.NextFloat(-0.25f, -0.1f) }, Projectile.Center);
                        Owner.PickAmmo(Owner.HeldItem, out _, out float shootSpeed, out int damage, out float knockback, out _);
                        if (Main.myPlayer == Projectile.owner)
                        {
                            for (int i = 0; i < Main.rand.Next(10, 14); i++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), shootPos, (shootVelocity * Main.rand.NextFloat(1f, 1.75f)).RotatedByRandom(0.15f), ModContent.ProjectileType<StellarStar>(), damage, knockback, Projectile.owner, ai1: Main.rand.NextBool() ? 1f : 0f);
                                if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm += 4.5f;
                            }
                        }
                        Timer = 0;
                        DrawTimer = 0;
                        StartedChargeLvl1 = true;
                        DecayCounter = NewMinCharge;
                        FiredProj = true;
                    }
                }

                if (!ChargeLvl3 && Timer >= NewMinCharge) DrawTimer++;
                if (!ChargeLvl3) Timer++;
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

            float opacity = Utils.GetLerpValue(0, NewMaxCharge, DrawTimer, true);
            auraColor = (HoldingRightClick && ChargeLvl3 ? MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * (StarCharge * 3f), colorList) : HoldingRightClick && ChargeLvl1 ? MogModUtils.MulticolorLerp((StarCharge * 1.5f), colorList) : Color.WhiteSmoke) * opacity * 0.8f;

            if (HoldingRightClick && ChargeLvl1)
            {
                if (MogClientConfig.Instance.GunRecoil)
                {
                    float rumble = MathHelper.Clamp(DrawTimer / (ChargeLvl3 ? 0.85f : ChargeLvl2 ? 1f : 1.2f), 0f, NewMaxCharge);
                    drawPosition += Main.rand.NextVector2Circular(rumble / 70f, rumble / 70f);
                }
            }
            if (Timer >= NewMinCharge)
            {
                for (int i = 0; i < 16; i++)
                {
                    Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/StellarBlasterGhost").Value;
                    Vector2 drawOffset = ((MathHelper.TwoPi * i / 16f).ToRotationVector2() * 5);
                    Main.EntitySpriteDraw(ghost, drawPosition + drawOffset, null, auraColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);
                }
            }
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale, flipSprite);
            return false;
        }
    }
}