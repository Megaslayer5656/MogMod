using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Config;
using MogMod.Items.Weapons.Ranged;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using Mono.Cecil;
using ReLogic.Content;
using System;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class DragonPiercerHoldout : BaseGunHoldoutProjectile
    {
        public static readonly SoundStyle WeakCharge = new($"{nameof(MogMod)}/Sounds/SE/bowChargeWeak")
        {
            Volume = 1.1f,
            PitchVariance = .2f,
            MaxInstances = 5
        };
        public static readonly SoundStyle StrongCharge = new($"{nameof(MogMod)}/Sounds/SE/bowChargeStrong")
        {
            Volume = 1.1f,
            PitchVariance = .2f,
            MaxInstances = 2
        };
        public override int AssociatedItemID => ModContent.ItemType<DragonPiercer>();
        private Asset<Texture2D> ItemTexture => TextureAssets.Item[AssociatedItemID];
        public override float MaxOffsetLengthFromArm => 22f;
        public ref float Time => ref Projectile.ai[0];
        public ref float ShotCounter => ref Projectile.ai[1];
        public ref float DrawTimer => ref Projectile.ai[2];
        public bool Charging = false;
        public float Cap = 10f;
        public float Spread = 0.1f;
        public int MaxShots = DragonPiercer.MaxShots;
        public int MinCharge = DragonPiercer.MinCharge;
        public int MaxCharge = DragonPiercer.MaxCharge;
        public override Vector2 GunTipPosition => Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.5f;
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
            if (Owner.CantUseHoldout() || HeldItem.type != AssociatedItemID)
            {
                Projectile.Kill();
            }
        }
        public override void HoldoutAI()
        {
            // TODO: change this so right click while channeling makes it fire 1, 3, 5 arrows; whereas regular left click makes it charge up to 5 arrows
            /// <summary>
            /// we want a bow that left click fires 1, 3, 5 arrows, or right click charge from 1 arrow up to 5 arrows
            /// 
            /// if left click, proj spawns, begin charging.
            /// once at max charge, fire arrow(s) (use shotCounter to track between 1, 3, 5), and reset charge
            /// 
            /// if right clicked, begin charge,
            /// new max charge is 2.5x regular max charge
            /// once timer % new max charge / 3 == 0, increase shotCounter
            /// if released, fire arrow(s), and reset timers
            /// </summary>
            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 20;
            var attackSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
            if (attackSpeed > Cap) attackSpeed = Cap;
            if (attackSpeed != 0f) attackSpeed = 1f / attackSpeed;
            int NewMinCharge = (int)(MinCharge * attackSpeed);
            int NewMaxCharge = (int)(MaxCharge * attackSpeed);

            Main.NewText($"timer = {Time}", Color.OldLace);
            if (Main.mouseLeft)
            {
                if (Time < NewMaxCharge) Time++;
                if (Time >= NewMinCharge)
                {
                    if (DrawTimer < NewMaxCharge) DrawTimer++;
                    if (Owner.MogMod().mouseRight)
                    {
                        if (Charging)
                        {
                            Time = 2;
                            DrawTimer = 0;
                            ShotCounter = 0;
                            Charging = false;
                        }
                        //Main.NewText($"timer = {Time}, drawtimer = {DrawTimer}", Color.OldLace);
                        if (Time % (NewMaxCharge / 3) == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item5 with { Volume = 0.3f, Pitch = 0.05f, PitchVariance = 0.1f, MaxInstances = -1 }, Projectile.Center);
                            Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, Vector2.Zero, 100, Color.DarkGoldenrod, Main.rand.NextFloat(0.8f, 1.2f));
                            for (int i = 0; i <= 12; i++)
                            {
                                Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, (shootVelocity * Main.rand.NextFloat(0.2f, 1.1f)).RotatedByRandom(0.4f), 0, default);
                                dust2.noGravity = true;
                                dust2.scale = Main.rand.NextFloat(0.8f, 1.4f);
                            }
                            if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm += 2f + ShotCounter; // visual recoil effect
                            Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int damage, out float knockback, out _);
                            if (Main.myPlayer == Projectile.owner)
                            {
                                //Main.NewText($"firing proj, shotcounter = {ShotCounter}", Color.Khaki);
                                DrawTimer = 0;
                                var source = Projectile.GetSource_FromThis();
                                int type = ModContent.ProjectileType<DragonPiercerArrow>();
                                if (ShotCounter >= 2) damage = (int)(damage * 1.5f);
                                SoundEngine.PlaySound(SoundID.Item38, Owner.Center);
                                Projectile.NewProjectile(source, GunTipPosition, shootVelocity, type, damage, knockback, Projectile.owner);
                                if (ShotCounter >= 1)
                                {
                                    Projectile.NewProjectile(source, GunTipPosition, shootVelocity.RotatedBy(Spread), type, damage, knockback, Projectile.owner);
                                    Projectile.NewProjectile(source, GunTipPosition, shootVelocity.RotatedBy(-Spread), type, damage, knockback, Projectile.owner);
                                }
                                if (ShotCounter >= 2)
                                {
                                    Projectile.NewProjectile(source, GunTipPosition, shootVelocity.RotatedBy(Spread * 2f), type, damage, knockback, Projectile.owner);
                                    Projectile.NewProjectile(source, GunTipPosition, shootVelocity.RotatedBy(-Spread * 2f), type, damage, knockback, Projectile.owner);
                                    Time = 2;
                                    ShotCounter = 0 - 1;
                                }
                            }
                            ShotCounter++;
                        }
                    }
                    else
                    {
                        if (!Owner.MogMod().mouseRight)
                        {
                            if (!Charging)
                            {
                                Time = 2;
                                DrawTimer = 0;
                                ShotCounter = 0;
                                Charging = true;
                            }
                            Main.NewText($"left clicking", Color.Bisque);
                        }
                        else if (Owner.MogMod().mouseRight || Time >= MaxCharge)
                        {
                            Main.NewText($"released left click", Color.IndianRed);
                            if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm += 5f + ShotCounter; // visual recoil effect
                            Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int damage, out float knockback, out _);
                            Time = 2;
                            DrawTimer = 0;
                            var source = Projectile.GetSource_FromThis();
                            int type = ModContent.ProjectileType<DragonPiercerArrow>();
                            SoundEngine.PlaySound(SoundID.Item38, Owner.Center);
                            Projectile.NewProjectile(source, GunTipPosition, shootVelocity, type, damage, knockback, Projectile.owner);
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

            var attackSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
            if (attackSpeed > Cap) attackSpeed = Cap;
            if (attackSpeed != 0f) attackSpeed = 1f / attackSpeed;
            int NewMinCharge = (int)(MinCharge * attackSpeed);
            int NewMaxCharge = (int)(MaxCharge * attackSpeed);
            
            Color[] colorList =
            [
                Color.PaleGoldenrod,
                Color.Goldenrod,
                new(255, 25, 75)
            ];
            if (Main.mouseLeft)
            {
                Color auraColor = Color.White;

                if (Owner.MogMod().mouseRight)
                {
                    float opacity = Utils.GetLerpValue(0, NewMaxCharge / 3, DrawTimer / 3, true);
                    //Main.NewText($"max charge = {MaxCharge}, new max charge = {NewMaxCharge}, opacity = {opacity}", Color.PaleGoldenrod);
                    float bolts = ShotCounter >= 2 ? 5 : ShotCounter >= 1 ? 3 : 1;
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
                        Color Transparency = Projectile.GetAlpha(lightColor) * (opacity * 2f);
                        var BoltTexture = ModContent.Request<Texture2D>("MogMod/Projectiles/RangedProjectiles/DragonPiercerArrow").Value;
                        Vector2 PointingTo = new((float)Math.Cos(Projectile.rotation + BoltAngle), (float)Math.Sin(Projectile.rotation + BoltAngle));
                        Vector2 ShiftDown = PointingTo.RotatedBy(-MathHelper.PiOver2);
                        float FlipFactor = Owner.direction < 0 ? MathHelper.Pi : 0f;
                        Vector2 boltPos = Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.25f - Main.screenPosition;
                        Vector2 drawOffset = ((MathHelper.TwoPi * i / 16f).ToRotationVector2() * 5);
                        Main.EntitySpriteDraw(BoltTexture, boltPos + drawOffset, null, Transparency, drawRotation + (BoltAngle * 1f) + MathHelper.PiOver2 + FlipFactor, BoltTexture.Size() * 0.5f, 1f, flipSprite, 0);
                    }
                    auraColor = (ShotCounter >= 2 ? new(255, 25, 75) : ShotCounter >= 1 ? Color.Goldenrod : Color.PaleGoldenrod) * opacity * 0.8f;
                }
                else if (!Owner.MogMod().mouseRight)
                {
                    float opacity = Utils.GetLerpValue(0, NewMaxCharge, DrawTimer, true);
                    auraColor = MogModUtils.MulticolorLerp(opacity * 0.8f, colorList);

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
            }
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale, flipSprite);
            return false;
        }
    }
}