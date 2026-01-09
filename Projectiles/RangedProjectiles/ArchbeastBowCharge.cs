using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Weapons.Ranged;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class ArchbeastBowCharge : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override string Texture => "MogMod/Items/Weapons/Ranged/ArchbeastParagon";
        public static readonly SoundStyle weakCharge = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/bowChargeWeak")
        {
            Volume = 1.1f,
            PitchVariance = .2f,
            MaxInstances = 5
        };
        public static readonly SoundStyle strongCharge = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/bowChargeStrong")
        {
            Volume = 1.1f,
            PitchVariance = .2f,
            MaxInstances = 2
        };
        private Player Owner => Main.player[Projectile.owner];
        private ref float CurrentChargingFrames => ref Projectile.ai[0];
        private ref float CurrentCharge => ref Projectile.ai[1];
        private ref float FramesToCharge => ref Projectile.localAI[0];

        private float storedVelocity = 1f;

        private float angularSpread = MathHelper.ToRadians(10);
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.5f;

            // If the player releases left click, shoot out the arrows
            if (Owner.CantUseHoldout())
            {

                if (CurrentCharge <= 0f) //If theres no arrows to shoot
                {
                    Projectile.Kill();
                    return;
                }
                // Fire the spread of arrows
                else
                    FireCharges(tipPosition);
            }

            else
            {
                // Frame 1 effects: Initialize the shoot speed
                if (FramesToCharge == 0f)
                {
                    FramesToCharge = Owner.ActiveItem().useAnimation;
                }

                if (Owner.HasAmmo(Owner.ActiveItem()))
                {
                    ++CurrentChargingFrames;

                    if (CurrentChargingFrames >= FramesToCharge && CurrentCharge < ArchbeastParagon.MaxCharge)
                    {
                        // Save the stats here for later
                        Item heldItem = Owner.ActiveItem();
                        Owner.PickAmmo(heldItem, out _, out float shootSpeed, out int damage, out float knockback, out _);
                        Projectile.damage = damage;
                        Projectile.knockBack = knockback;
                        storedVelocity = shootSpeed;

                        CurrentChargingFrames = 0f;
                        ++CurrentCharge;

                        FramesToCharge *= 0.950f;

                        if (CurrentCharge >= ArchbeastParagon.MaxCharge)
                        {
                            Projectile.damage *= 2;
                            SoundEngine.PlaySound(strongCharge);
                            for (int i = 0; i < 75; i++)
                            {
                                float colorRando = Main.rand.NextFloat(0, 1);
                                float offsetAngle = MathHelper.TwoPi * i / 75f;

                                // makes an asteroid shape (not the big evil rock from space)
                                float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 3D);
                                float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 3D);

                                Vector2 puffDustVelocity = new Vector2(unitOffsetX, unitOffsetY) * 5f;
                                Dust charged = Dust.NewDustPerfect(tipPosition, 267, puffDustVelocity);
                                charged.scale = 1.5f;
                                charged.fadeIn = 0.5f;
                                charged.color = Color.Lerp(Color.PaleVioletRed, Color.MediumVioletRed, colorRando);
                                charged.noGravity = true;
                            }
                        }
                        else
                        {
                            SoundEngine.PlaySound(weakCharge);
                            for (int i = 0; i < 75; i++)
                            {
                                float colorRando = Main.rand.NextFloat(0, 1);
                                float offsetAngle = MathHelper.TwoPi * i / 75f;
                                // Parametric equations for an asteroid.
                                float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 3D);
                                float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 3D);

                                Vector2 puffDustVelocity = new Vector2(unitOffsetX, unitOffsetY) * 2.5f;
                                Dust charged = Dust.NewDustPerfect(tipPosition, 267, puffDustVelocity);
                                charged.scale = 0.75f;
                                charged.fadeIn = 0.5f;
                                charged.color = Color.Lerp(Color.LightYellow, Color.LightGoldenrodYellow, colorRando);
                                charged.noGravity = true;
                            }
                        }
                    }
                }
            }

            UpdateProjectileHeldVariables(armPosition);
            ManipulatePlayerVariables();
        }

        public void FireCharges(Vector2 tipPosition)
        {
            // ignore extra spread from additional charges past max
            if (CurrentCharge == ArchbeastParagon.MaxCharge)
            {
                for (int i = 0; i < CurrentCharge; i++)
                {
                    //Version that doesnt inclue the chargingframes bit , since its fully charged
                    float increment = angularSpread * (CurrentCharge - 1) / 2;
                    float spreadForThisProjectile = MathHelper.Lerp(-increment, increment, i / (float)(CurrentCharge - 1));
                    ShootProjectiles(tipPosition, spreadForThisProjectile);

                }
                SoundEngine.PlaySound(SoundID.Item38); // shotgun boom
            }
            else if (CurrentCharge != 0)
            {
                for (int i = 0; i < CurrentCharge + 1; i++)
                {
                    if (i == CurrentCharge) //We don't actually shoot the arrow that's currently loading
                        continue;

                    //Version that takes into account the progress of the arrow currently loading. It takes half the time it takes for the arrow to load for the range to adjust to its next position
                    float increment = angularSpread * (CurrentCharge - 1 + MathHelper.Clamp((CurrentChargingFrames / FramesToCharge) * 2, 0f, 1f)) / 2;
                    float spreadForThisProjectile = MathHelper.Lerp(-increment, increment, i / (float)(MathHelper.Lerp(CurrentCharge - 1, CurrentCharge, MathHelper.Clamp((CurrentChargingFrames * 2 / FramesToCharge), 0f, 1f))));
                    ShootProjectiles(tipPosition, spreadForThisProjectile);
                }
                SoundEngine.PlaySound(SoundID.Item38); // shotgun boom
            }

            FramesToCharge = Owner.ActiveItem().useAnimation; //reset the reload time
            CurrentCharge = 0; //Unload the bow
        }

        public void ShootProjectiles(Vector2 tipPosition, float projectileRotation)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(projectileRotation) * storedVelocity;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, shootVelocity, ModContent.ProjectileType<DragonPiercerArrow>(), Projectile.damage, Projectile.knockBack * 2, Projectile.owner);
        }

        private void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float interpolant = Utils.GetLerpValue(5f, 25f, Owner.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Owner.SafeDirectionTo(Main.MouseWorld), interpolant);
                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.position = armPosition - Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.Zero) * 35f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            int oldDirection = Projectile.spriteDirection;
            if (oldDirection == -1)
                Projectile.rotation += MathHelper.Pi;

            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
            // If the direction differs from what it originaly was, undo the previous 180 degree turn.
            // If this is not done, the bow will have 1 frame of rotational "jitter" when the direction changes based on the
            // original angle. This effect looks very strange in-game.
            if (Projectile.spriteDirection != oldDirection)
                Projectile.rotation -= MathHelper.Pi;

            Projectile.timeLeft = 3;
        }

        private void ManipulatePlayerVariables()
        {
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }

        // cool arrow draw effect from calamity clockwerk chainmeal bow
        public override bool PreDraw(ref Color lightColor)
        {
            float loops = CurrentCharge + 1;
            if (CurrentCharge == ArchbeastParagon.MaxCharge)
                loops = CurrentCharge;

            for (int i = 0; i < loops; i++)
            {
                float BoltAngle;
                float Shift = 0;

                if (CurrentCharge == 0)
                {
                    BoltAngle = 0;
                }
                else if (CurrentCharge == ArchbeastParagon.MaxCharge)
                {
                    float increment = angularSpread * (CurrentCharge - 1) / 2;
                    BoltAngle = MathHelper.Lerp(-increment, increment, i / (float)(CurrentCharge - 1));
                }
                else
                {
                    float increment = angularSpread * (CurrentCharge - 1 + MathHelper.Clamp((CurrentChargingFrames * 2 / FramesToCharge), 0f, 1f)) / 2;
                    BoltAngle = MathHelper.Lerp(-increment, increment, i / (float)(MathHelper.Lerp(CurrentCharge - 1, CurrentCharge, MathHelper.Clamp((CurrentChargingFrames * 2 / FramesToCharge), 0f, 1f))));
                }

                if (i == CurrentCharge)
                    Shift = 1 - (CurrentChargingFrames / FramesToCharge);

                Color Transparency = Color.White * (1 - Shift);
                var BoltTexture = ModContent.Request<Texture2D>("MogMod/Projectiles/RangedProjectiles/DragonPiercerArrow").Value;
                Vector2 PointingTo = new Vector2((float)Math.Cos(Projectile.rotation + BoltAngle), (float)Math.Sin(Projectile.rotation + BoltAngle));
                Vector2 ShiftDown = PointingTo.RotatedBy(-MathHelper.PiOver2);
                float FlipFactor = Owner.direction < 0 ? MathHelper.Pi : 0f;
                Vector2 drawPosition = Owner.Center + PointingTo.RotatedBy(FlipFactor) * (10f + (Shift * 40)) - ShiftDown.RotatedBy(FlipFactor) * (BoltTexture.Width / 2) - Main.screenPosition;

                Main.EntitySpriteDraw(BoltTexture, drawPosition, null, Transparency, Projectile.rotation + (BoltAngle * 1f) + MathHelper.PiOver2 + FlipFactor, BoltTexture.Size(), 1f, 0, 0);
            }
            return true;
        }
        public override bool? CanDamage() => false;
    }
}