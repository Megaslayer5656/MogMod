using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
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

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class WyvernJawbladeHoldout : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override string Texture => "MogMod/Items/Weapons/Melee/WyvernJawblade";
        private ref float CurrentCharge => ref Projectile.ai[0];
        public bool canAttack = false;
        public bool initialized = false;
        public float chargeDamage = 0f;
        private readonly float[] amount = [30f, 60f, 100f];
        private const float swingRange = 1.67f * (float)Math.PI;
        private const float firstHalfSwing = .45f;
        private const float windUp = 0.15f;
        private const float unwind = 0.2f;
        private float prepTime => 24f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float execTime => 16f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float hideTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private Player Owner => Main.player[Projectile.owner];
        private enum AttackStage
        {
            Prepare,
            Execute,
            Unwind
        }
        private AttackStage CurrentStage
        {
            get => (AttackStage)Projectile.localAI[0];
            set
            {
                Projectile.localAI[0] = (float)value;
                Timer = 0;
            }
        }
        private ref float InitialAngle => ref Projectile.ai[1];
        private ref float Timer => ref Projectile.ai[2];
        private ref float Progress => ref Projectile.localAI[1];
        private ref float Size => ref Projectile.localAI[2];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 88;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void OnSpawn(IEntitySource source)
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
            InitialAngle = targetAngle - firstHalfSwing * swingRange * Projectile.spriteDirection;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((sbyte)Projectile.spriteDirection);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadSByte();
        }
        public override void AI()
        {
            Owner.itemAnimation = 2;
            Owner.itemTime = 2;
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }
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
                if (!Owner.CantUseHoldout())
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
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                        Projectile.netUpdate = true;
                    }
                    InitialAngle = targetAngle - firstHalfSwing * swingRange * Projectile.spriteDirection;
                    Owner.ChangeDir(Projectile.direction);
                    Owner.velocity.X *= .95f;
                    if (CurrentCharge <= WyvernJawblade.MaxCharge)
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
                    SoundEngine.PlaySound(SoundID.Item20 with { Pitch = CurrentCharge >= WyvernJawblade.MaxCharge ? -0.2f : 0.1f });
                    int dustAmt = CurrentCharge == WyvernJawblade.MaxCharge ? 20 : 8;
                    for (int j = 0; j < dustAmt; j++)
                    {
                        Vector2 dustRotate = new Vector2((float)Owner.width / 2f, (float)Owner.height) * 0.1f;
                        dustRotate = dustRotate.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Owner.Center;
                        Vector2 dustDirection = dustRotate - Owner.Center;
                        int killDust = Dust.NewDust(dustRotate + dustDirection, 0, 0, DustID.AncientLight, dustDirection.X, dustDirection.Y, 100, CurrentCharge >= WyvernJawblade.MaxCharge ? Color.Goldenrod : Color.LightGoldenrodYellow, 1.2f);
                        Main.dust[killDust].noGravity = true;
                        Main.dust[killDust].velocity = dustDirection;
                    }
                }
            }
        }
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
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }
        public override void CutTiles()
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
        }
        public override bool? CanDamage()
        {
            if (!canAttack)
                return false;
            return base.CanDamage();
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;
            modifiers.SourceDamage *= (chargeDamage / 25f) + 1f;
            modifiers.Knockback += (chargeDamage / 100f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.damage = (int)(Projectile.damage * .9f);
        public void SetSwordPosition()
        {
            Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f));
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2);
            if (Owner.gravDir == -1f)
            {
                Projectile.rotation = 0f - Projectile.rotation;
                armPosition.Y = Owner.Bottom.Y + (Owner.position.Y - armPosition.Y);
            }
            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition;
            Projectile.scale = Size * 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem);
            Owner.heldProj = Projectile.whoAmI;
        }
        private void PrepareStrike()
        {
            Timer++;
            Progress = windUp * swingRange * (1f - Timer / prepTime);
            Size = MathHelper.SmoothStep(0, 1, Timer / prepTime);
            if (Timer >= prepTime)
                CurrentStage = AttackStage.Execute;
        }
        private void ExecuteStrike()
        {
            float t = Timer / execTime;
            float easing = (float)Math.Sin(t * MathHelper.PiOver2);
            Progress = MathHelper.Lerp(0, swingRange, easing);
            if (canAttack && !initialized)
            {
                SoundEngine.PlaySound(SoundID.Item1 with { Pitch = CurrentCharge >= WyvernJawblade.MaxCharge ? -0.3f : -0.15f }, Projectile.Center);
                initialized = true;
            }
            if (Main.rand.NextBool(3))
            {
                Vector2 dustCorner = Owner.position - 2f * Vector2.One;
                Vector2 dustVel = Owner.velocity + new Vector2(0f, Main.rand.NextFloat(-5f, -1f));
                int d = Dust.NewDust(dustCorner, Owner.width, Owner.height, DustID.SandSpray, dustVel.X, dustVel.Y);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity.Y -= 1.5f;
                Main.dust[d].scale = 0.8f;
                Main.dust[d].fadeIn = Main.rand.NextFloat(0.6f, 0.8f);
            }
            if (Timer >= execTime)
                CurrentStage = AttackStage.Unwind;
        }
        private void UnwindStrike()
        {
            float t = Timer / hideTime;
            float easing = (float)Math.Sin(t * MathHelper.PiOver2);
            Size = 1f - easing;
            if (Timer >= hideTime)
                Projectile.Kill();
        }
    }
}