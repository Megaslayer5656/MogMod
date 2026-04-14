using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    // what the FUCK am i doing
    public class AnchorHoldout : ModProjectile, ILocalizedModType
    {
        // taken from example mod custom swing sword
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override string Texture => "MogMod/Items/Weapons/Melee/OversizedAnchor";

        // We define some constants that determine the swing range of the sword
        // Not that we use multipliers here since that simplifies the amount of tweaks for these interactions
        // You could change the values or even replace them entirely, but they are tweaked with looks in mind
        private const float swingRange = 1.67f * (float)Math.PI; // The angle a swing attack covers (300 deg)
        private const float firstHalfSwing = .45f; // How much of the swing happens before it reaches the target angle (in relation to swingRange)
        private const float windUp = 0.15f; // How far back the player's hand goes when winding their attack (in relation to swingRange)
        private const float unwind = 0.2f; // When should the sword start disappearing

        private const float SPINRANGE = 2.5f * (float)Math.PI;
        private const float SPINTIME = 1f; // How much longer a spin is than a swing

        public bool hitGoon = false;
        public bool initialized = false;

        // We define timing functions for each stage, taking into account melee attack speed
        // Note that you can change this to suit the need of your projectile
        private float prepTime => 10f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float execTime => 30f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float hideTime => 10f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private Player Owner => Main.player[Projectile.owner];

        private enum AttackType // Which attack is being performed
        {
            // Swings are normal sword swings that can be slightly aimed
            // Swings goes through the full cycle of animations
            SwingUp,
            Slam,
            Spin,
        }
        private enum AttackStage // What stage of the attack is being executed, see functions found in AI for description
        {
            Prepare,
            Execute,
            Unwind
        }

        // These properties wrap the usual ai and localAI arrays for cleaner and easier to understand code.
        private AttackType CurrentAttack
        {
            get => (AttackType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        private AttackStage CurrentStage
        {
            get => (AttackStage)Projectile.localAI[0];
            set
            {
                Projectile.localAI[0] = (float)value;
                Timer = 0; // reset the timer when the projectile switches states
            }
        }

        // Variables to keep track of during runtime
        private ref float InitialAngle => ref Projectile.ai[1]; // Angle aimed in (with constraints)
        private ref float Timer => ref Projectile.ai[2]; // Timer to keep track of progression of each stage
        private ref float Progress => ref Projectile.localAI[1]; // Position of sword relative to initial angle
        private ref float Size => ref Projectile.localAI[2]; // Size of sword


        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true; // Uses local immunity frames
            Projectile.localNPCHitCooldown = -1; // We set this to -1 to make sure the projectile doesn't hit twice
            Projectile.ownerHitCheck = true; // Make sure the owner of the projectile has line of sight to the target (aka can't hit things through tile).
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            float targetAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation();
            if (Projectile.spriteDirection == 1)
            {
                // However, we limit the rangle of possible directions so it does not look too ridiculous
                targetAngle = MathHelper.Clamp(targetAngle, (float)-Math.PI * 1 / 3, (float)Math.PI * 1 / 6);
            }
            else
            {
                if (targetAngle < 0)
                {
                    targetAngle += 2 * (float)Math.PI; // This makes the range continuous for easier operations
                }

                targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 5 / 6, (float)Math.PI * 4 / 3);
            }

            InitialAngle = targetAngle - firstHalfSwing * swingRange * Projectile.spriteDirection; // Otherwise, we calculate the angle
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            // Projectile.spriteDirection for this projectile is derived from the mouse position of the owner in OnSpawn, as such it needs to be synced. spriteDirection is not one of the fields automatically synced over the network. All Projectile.ai slots are used already, so we will sync it manually.
            writer.Write((sbyte)Projectile.spriteDirection);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadSByte();
        }
        public override void AI()
        {
            // Extend use animation until projectile is killed
            Owner.itemAnimation = 2;
            Owner.itemTime = 2;

            // Kill the projectile if the player dies or gets crowd controlled
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }

            // AI depends on stage and attack
            // Note that these stages are to facilitate the scaling effect at the beginning and end
            // If this is not desirable for you, feel free to simplify
            switch (CurrentStage)
            {
                case AttackStage.Prepare:
                    PrepareStrike();
                    break;
                case AttackStage.Execute:
                    ExecuteStrike();
                    break;
                default:
                    UnwindStrike();
                    break;
            }

            SetSwordPosition();
            Timer++;
        }

        // Calculate origin of sword (hilt) based on orientation and offset sword rotation (as sword is angled in its sprite)
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(0, Projectile.height);
                rotationOffset = MathHelper.ToRadians(45f);
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(Projectile.width, Projectile.height);
                rotationOffset = MathHelper.ToRadians(135f);
                effects = SpriteEffects.FlipHorizontally;
            }

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

            // Since we are doing a custom draw, prevent it from normally drawing
            return false;
        }

        // Find the start and end of the sword and use a line collider to check for collision with enemies
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }

        // Do a similar collision check for tiles
        public override void CutTiles()
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
        }

        // We make it so that the projectile can only do damage in its release and unwind phases
        public override bool? CanDamage()
        {
            if (CurrentStage == AttackStage.Prepare)
                return false;
            return base.CanDamage();
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Make knockback go away from player
            modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;
            if (CurrentAttack == AttackType.Slam)
            {
                modifiers.Knockback += 1;
                modifiers.SourceDamage *= 1.5f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => hitGoon = true;

        // Function to easily set projectile and arm position
        public void SetSwordPosition()
        {
            Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress; // Set projectile rotation

            // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

            // Adjust the position for reversed gravity.
            if (Owner.gravDir == -1f)
            {
                Projectile.rotation = 0f - Projectile.rotation;
                armPosition.Y = Owner.Bottom.Y + (Owner.position.Y - armPosition.Y);
            }

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition; // Set projectile to arm position
            Projectile.scale = Size * 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem); // Slightly scale up the projectile and also take into account melee size modifiers

            Owner.heldProj = Projectile.whoAmI; // set held projectile to this projectile
        }

        // Function facilitating the taking out of the sword
        private void PrepareStrike()
        {
            // first upwards swing effect
            if (CurrentAttack == AttackType.SwingUp)
            {
                Progress = windUp / 2 * (swingRange / 2) * (1f - Timer / (prepTime / 1.4f)); // Calculates rotation from initial angle
                Size = MathHelper.SmoothStep(0, 1, Timer / (prepTime / 1.4f)); // Make sword slowly increase in size as we prepare to strike until it reaches max

                if (Timer >= (prepTime / 1.4f))
                {
                    initialized = true;
                    // Play sword sound here since playing it on spawn is too early
                    SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFurySwing, Projectile.Center);
                    CurrentStage = AttackStage.Execute; // If attack is over prep time, we go to next stage
                }
            }
            // first slam effect
            else if (CurrentAttack == AttackType.Slam)
            {
                Progress = windUp * swingRange * (1f - Timer / prepTime); // Calculates rotation from initial angle
                Size = MathHelper.SmoothStep(0, 1, Timer / prepTime); // Make sword slowly increase in size as we prepare to strike until it reaches max

                if (Timer >= prepTime)
                {
                    initialized = true;
                    // Play sword sound here since playing it on spawn is too early
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, Projectile.Center);
                    CurrentStage = AttackStage.Execute; // If attack is over prep time, we go to next stage
                }
            }
            else if (CurrentAttack == AttackType.Spin)
            {
                Progress = windUp / 2 * (swingRange / 2) * (1f - Timer / prepTime);
                Size = MathHelper.SmoothStep(0, 1, Timer / prepTime);
                if (Timer >= prepTime)
                {
                    SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFurySwing);
                    CurrentStage = AttackStage.Execute;
                }
            }
        }

        // Function facilitating the first half of the swing
        private void ExecuteStrike()
        {
            var mogPlayerUI = Main.LocalPlayer.GetModPlayer<MogPlayerUI>();
            if (CurrentAttack == AttackType.SwingUp)
            {
                Progress = MathHelper.SmoothStep(0, -swingRange, (1f - unwind) * Timer / (execTime / 1.4f));

                // shoot out a dolphin
                if ((Timer >= (execTime / 1.4f) / 1.5f) && initialized == true)
                {
                    initialized = false;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitY) * 10f, ModContent.ProjectileType<AnchorProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }

                // fire dolphins if hit npc
                if (hitGoon)
                {
                    hitGoon = false;
                    bool randomBool = Main.rand.Next(2) == 0;
                    for (int i = 0; i < 3; i++)
                        MogModUtils.ProjectileBarrage(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.Center, randomBool, 200f, 200f, -200f, 200f, 6f, ModContent.ProjectileType<AnchorProj>(), Convert.ToInt32(Projectile.damage * .45), 3f, Projectile.owner, false, 0f);
                }

                if (Timer >= (execTime / 1.4f))
                    CurrentStage = AttackStage.Unwind;
            }
            else if (CurrentAttack == AttackType.Slam)
            {
                float t = Timer / execTime;
                float easing = (float)Math.Sin(t * MathHelper.PiOver2);
                Progress = MathHelper.Lerp(0, swingRange, easing);

                // shoot out a dolphin
                if ((Timer >= (execTime / 1.4f) / 1.5f) && initialized == true)
                {
                    initialized = false;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitY) * 10f, ModContent.ProjectileType<AnchorProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }

                // fire dolphins if hit npc
                if (hitGoon)
                {
                    hitGoon = false;
                    bool randomBool = Main.rand.Next(2) == 0;
                    for (int i = 0; i < 3; i++)
                        MogModUtils.ProjectileBarrage(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.Center, randomBool, 200f, 200f, -200f, 200f, 6f, ModContent.ProjectileType<AnchorProj>(), Convert.ToInt32(Projectile.damage * .45), 3f, Projectile.owner, false, 0f);
                }
                
                if (Timer >= execTime)
                    CurrentStage = AttackStage.Unwind;
            }
            else if (CurrentAttack == AttackType.Spin)
            {
                Progress = MathHelper.SmoothStep(0, -SPINRANGE, (1f - unwind / 2) * Timer / (execTime * SPINTIME));
                if (hitGoon)
                {
                    hitGoon = false;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AnchorSmashProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                if (Timer >= execTime * SPINTIME)
                    CurrentStage = AttackStage.Unwind;
            }
        }

        // Function facilitating the latter half of the swing where the sword unwinds
        private void UnwindStrike()
        {
            if (CurrentAttack == AttackType.SwingUp)
            {
                Progress = MathHelper.SmoothStep(-swingRange, 0, (1f - unwind) * Timer / (execTime / 1.4f));
                Size = 1f - MathHelper.SmoothStep(0, 1, Timer / (hideTime / 1.4f)); // Make sword slowly decrease in size as we end the swing to make a smooth hiding animation

                if (Timer >= (hideTime / 1.4f))
                    Projectile.Kill();
            }
            else if (CurrentAttack == AttackType.Slam)
            {
                float t = Timer / hideTime;
                float easing = (float)Math.Sin(t * MathHelper.PiOver2);
                Size = 1f - easing;
                if (Timer >= hideTime)
                    Projectile.Kill();
            }
            else if (CurrentAttack == AttackType.Spin)
            {
                Progress = MathHelper.SmoothStep(-swingRange, 0, (1f - unwind / 2) + unwind / 2 * Timer / (hideTime * SPINTIME / 2));
                Size = 1f - MathHelper.SmoothStep(0, 1, Timer / (hideTime * SPINTIME / 2));
                if (Timer >= hideTime * SPINTIME / 2)
                {
                    CurrentAttack = AttackType.SwingUp;
                    Projectile.Kill();
                }
            }
        }
    }
}