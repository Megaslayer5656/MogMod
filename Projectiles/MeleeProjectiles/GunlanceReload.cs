using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Weapons.Melee;
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
    // all of this could of been done in gunlanceholdout but it was easier to do it here
    public class GunlanceReload : ModProjectile, ILocalizedModType
    {
        // taken from gunlanceholdout, which was taken from example mod custom swing sword
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override string Texture => "MogMod/Items/Weapons/Melee/Gunlance";
        private const float swingRange = 1.67f * (float)Math.PI;
        private const float firstHalfSwing = .45f;
        private const float windUp = 0.15f;
        private const float unwind = 0.2f;
        private float prepTime => 24f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float execTime => 20f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float hideTime => 24f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
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
            Projectile.width = 94;
            Projectile.height = 90;
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
        public override void SendExtraAI(BinaryWriter writer) => writer.Write((sbyte)Projectile.spriteDirection);
        public override void ReceiveExtraAI(BinaryReader reader) => Projectile.spriteDirection = reader.ReadSByte();
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
            Timer++;
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
        public override bool? CanDamage() => false;
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
            Progress = windUp * swingRange * (1f - Timer / (prepTime / 2));
            Size = MathHelper.SmoothStep(0, 1, Timer / (prepTime / 2));
            if (Timer >= (prepTime / 2))
            {
                SoundEngine.PlaySound(SoundID.Item149, Projectile.Center);
                CurrentStage = AttackStage.Execute;
            }
        }
        private void ExecuteStrike()
        {
            var mogPlayerUI = Main.LocalPlayer.GetModPlayer<MogPlayerUI>();
            SoundEngine.PlaySound(SoundID.Item23);
            float t = Timer / (execTime / 2f);
            float easing = (float)Math.Sin(t * MathHelper.PiOver4);
            Progress = MathHelper.Lerp(0, (swingRange / 2f), easing);
            if (Timer >= (execTime / 2f))
                CurrentStage = AttackStage.Unwind;
        }
        private void UnwindStrike()
        {
            float t = Timer / hideTime / 2;
            float easing = (float)Math.Sin(t * MathHelper.PiOver4);
            Size = 1f - easing;
            if (Timer >= hideTime / 2)
            {
                Gunlance.Blast = true;
                Projectile.Kill();
            }
        }
    }
}