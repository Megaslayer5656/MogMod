using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using MogMod.Buffs.Debuffs;
using MogMod.Items.Weapons.Melee;
using MogMod.Utilities;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class BlackBladeHoldout : ModProjectile, ILocalizedModType
    {
        // taken from example mod custom swing sword
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "MogMod/Items/Weapons/Melee/BlackBlade";
        private ref float CurrentCharge => ref Projectile.ai[0];
        public bool canAttack = false;
        public bool initialized = false;
        public float chargeDamage = 0f;
        private readonly float[] amount = [30f, 60f, BlackBlade.MaxCharge];
        private const float swingRange = 1.67f * (float)Math.PI; // The angle a swing attack covers (300 deg)
        private const float firstHalfSwing = .45f; // How much of the swing happens before it reaches the target angle (in relation to swingRange)
        private const float windUp = 0.15f; // How far back the player's hand goes when winding their attack (in relation to swingRange)
        private const float unwind = 0.2f; // When should the sword start disappearing

        // We define timing functions for each stage, taking into account melee attack speed
        // Note that you can change this to suit the need of your projectile
        private float prepTime => 24f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float execTime => 22f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float hideTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private Player Owner => Main.player[Projectile.owner];
        private enum AttackStage // What stage of the attack is being executed, see functions found in AI for description
        {
            Prepare,
            Execute,
            Unwind
        }

        // These properties wrap the usual ai and localAI arrays for cleaner and easier to understand code.
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
            Projectile.width = Projectile.height = 114;
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
            if (CurrentStage != AttackStage.Prepare)
            {
                if (!Owner.CantUseHoldout() && Projectile.owner == Main.myPlayer)
                {
                    Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
                    float targetAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation();
                    if (Projectile.spriteDirection == 1)
                        targetAngle = MathHelper.Clamp(targetAngle, (float)-Math.PI * 1 / 3, (float)Math.PI * 1 / 6);
                    else
                    {
                        if (targetAngle < 0)
                            targetAngle += 2 * (float)Math.PI;
                        targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 5 / 6, (float)Math.PI * 4 / 3);
                    }
                    Projectile.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                    Projectile.netUpdate = true;
                    InitialAngle = targetAngle - firstHalfSwing * swingRange * Projectile.spriteDirection;
                    Owner.ChangeDir(Projectile.direction);
                    if (CurrentCharge <= BlackBlade.MaxCharge)
                        CurrentCharge++;
                }
                else
                {
                    canAttack = true;
                    Timer++;
                }
                if (amount.Contains(CurrentCharge))
                {
                    chargeDamage = CurrentCharge;
                    SoundEngine.PlaySound(SoundID.Item20 with { Pitch = CurrentCharge >= BlackBlade.MaxCharge ? -0.3f : -0.1f});
                    int dustAmt = CurrentCharge == BlackBlade.MaxCharge ? 20 : 8;
                    for (int j = 0; j < dustAmt; j++)
                    {
                        Vector2 dustRotate = new Vector2((float)Owner.width / 2f, (float)Owner.height) * 0.1f;
                        dustRotate = dustRotate.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Owner.Center;
                        Vector2 dustDirection = dustRotate - Owner.Center;
                        int killDust = Dust.NewDust(dustRotate + dustDirection, 0, 0, DustID.AncientLight, dustDirection.X, dustDirection.Y, 100, CurrentCharge >= BlackBlade.MaxCharge ? Color.PaleVioletRed : Color.LightGoldenrodYellow, 1.2f);
                        Main.dust[killDust].noGravity = true;
                        Main.dust[killDust].velocity = dustDirection;
                    }
                }
            }
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
            if (!canAttack)
                return false;
            return base.CanDamage();
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Make knockback go away from player
            modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;
            modifiers.SourceDamage *= (chargeDamage / (CurrentCharge >= BlackBlade.MaxCharge ? 20.833f : 50f)) + 1f;
            modifiers.Knockback += (chargeDamage / 25f);
            if (target.life >= (int)(target.lifeMax * .9f))
                modifiers.FinalDamage *= 1.5f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BlackBladeDebuff>(), CurrentCharge >= BlackBlade.MaxCharge ? 300 : 90);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<BlackBladeDebuff>(), CurrentCharge >= BlackBlade.MaxCharge ? 300 : 90);

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
            Timer++;
            // first slam effect
            Progress = windUp * swingRange * (1f - Timer / prepTime); // Calculates rotation from initial angle
            Size = MathHelper.SmoothStep(0, 1, Timer / prepTime); // Make sword slowly increase in size as we prepare to strike until it reaches max

            if (Timer >= prepTime)
            {
                CurrentStage = AttackStage.Execute; // If attack is over prep time, we go to next stage
            }
        }

        // Function facilitating the first half of the swing
        private void ExecuteStrike()
        {
            float t = Timer / execTime;
            float easing = (float)Math.Sin(t * MathHelper.PiOver2);
            Progress = MathHelper.Lerp(0, swingRange, easing);
            if (canAttack && !initialized)
            {
                SoundEngine.PlaySound(SoundID.Item1 with { Pitch = CurrentCharge >= BlackBlade.MaxCharge ? -0.3f : -0.15f }, Projectile.Center);
                initialized = true;
            }

            if (Main.rand.NextBool(3))
            {
                Vector2 dustCorner = Owner.position - 2f * Vector2.One;
                Vector2 dustVel = Owner.velocity + new Vector2(0f, Main.rand.NextFloat(-5f, -1f));
                int d = Dust.NewDust(dustCorner, Owner.width, Owner.height, DustID.CrimsonSpray, dustVel.X, dustVel.Y);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity.Y -= 1.5f;
                Main.dust[d].scale = 0.8f;
                Main.dust[d].fadeIn = Main.rand.NextFloat(0.6f, 0.8f);
            }
            if (Timer >= execTime)
            {
                CurrentStage = AttackStage.Unwind;
            }
        }

        // Function facilitating the latter half of the swing where the sword unwinds
        private void UnwindStrike()
        {
            float t = Timer / hideTime;
            float easing = (float)Math.Sin(t * MathHelper.PiOver2);
            Size = 1f - easing;
            if (Timer >= hideTime)
            {
                Projectile.Kill();
            }
        }
    }
}