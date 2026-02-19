using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Ammo;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static MogMod.Utilities.MiscUtils;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class CarianSlicerProj : ModProjectile, ILocalizedModType
    {
        // code taken from terratomere calamity mod
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override string Texture => "MogMod/Items/Weapons/Magic/MichaelSword";

        public Player Owner => Main.player[Projectile.owner];

        public int Direction => Projectile.velocity.X.DirectionalSign();

        public float SwingCompletion => MathHelper.Clamp(Time / CarianSlicer.SwingTime, 0f, 1f);

        public float SwingCompletionAtStartOfTrail
        {
            get
            {
                float swingCompletion = SwingCompletion - CarianSlicer.TrailOffsetCompletionRatio;

                // Ensure that the trail does not attempt to "start" in the anticipation state, as the trail only exists after the charge begins.
                return MathHelper.Clamp(swingCompletion, SwingCompletionRatio, 1f);
            }
        }

        public float SwordRotation
        {
            get
            {
                float swordRotation = InitialRotation + GetSwingOffsetAngle(SwingCompletion) * Projectile.spriteDirection + MathHelper.PiOver4;
                if (Projectile.spriteDirection == -1)
                    swordRotation += MathHelper.PiOver2;
                return swordRotation;
            }
        }

        public Vector2 SwordDirection => SwordRotation.ToRotationVector2() * Direction;

        public ref float Time => ref Projectile.ai[0];

        public ref float InitialRotation => ref Projectile.ai[1];

        // Easings for things such as rotation.
        public static float SwingCompletionRatio => 0.27f;

        public static float RecoveryCompletionRatio => 0.84f;

        // Brief delay before the animations begin, with the blade simply being held upright for a time.
        public static CurveSegment AnticipationWait => new(EasingType.PolyOut, 0f, -1.67f, 0f);

        // Period of time where the blade reels back in anticipation of a swing.
        public static CurveSegment Anticipation => new(EasingType.PolyOut, 0.14f, AnticipationWait.EndingHeight, -.05f, 1);

        // A short, powerful swing that rapidly approaches it destination.
        public static CurveSegment Swing => new(EasingType.PolyIn, SwingCompletionRatio, Anticipation.EndingHeight, 3f, 5);

        // Period of time after the swing where the blade reels back further before it disappears.
        public static CurveSegment Recovery => new(EasingType.PolyOut, RecoveryCompletionRatio, Swing.EndingHeight, 0.97f, 3);

        public static float GetSwingOffsetAngle(float completion) => PiecewiseAnimation(completion, AnticipationWait, Anticipation, Swing, Recovery);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 100;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 66;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = CarianSlicer.SwingTime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.noEnchantmentVisuals = true;
        }

        #region AI and Behaviors

        public override void AI()
        {
            // Initialize the initial rotation if necessary.
            if (InitialRotation == 0f)
            {
                InitialRotation = Projectile.velocity.ToRotation();
                Projectile.netUpdate = true;
            }

            // Perform squish effects.
            Projectile.scale = Utils.GetLerpValue(0f, 0.13f, SwingCompletion, true) * Utils.GetLerpValue(1f, 0.87f, SwingCompletion, true) * 0.7f + 0.3f;

            AdjustPlayerValues();
            StickToOwner();
            CreateProjectiles();
            if (SwingCompletion > SwingCompletionRatio + 0.2f && SwingCompletion < RecoveryCompletionRatio)
                CreateSlashSparkleDust();

            // Determine rotation.
            Projectile.rotation = SwordRotation;
            Time++;
        }

        public void AdjustPlayerValues()
        {
            Projectile.spriteDirection = Projectile.direction = Direction;
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.direction * Projectile.velocity).ToRotation();

            // Decide the arm rotation for the owner.
            float armRotation = SwordRotation - Direction * 1.67f;
            Owner.SetCompositeArmFront(Math.Abs(armRotation) > 0.01f, Player.CompositeArmStretchAmount.Full, armRotation);
        }

        public void StickToOwner()
        {
            // Glue the sword to its owner. This applies a handful of offsets to make the blade look like it's roughly inside of the owner's hand.
            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter, true) + SwordDirection * new Vector2(7f, 16f) * Projectile.scale;
            Projectile.Center -= Projectile.velocity.SafeNormalize(Vector2.UnitY) * new Vector2(66f, 54f + Projectile.scale * 8f);

            // Set the owner's held projectile to this and register a false item time calculation.
            Owner.heldProj = Projectile.whoAmI;
            Owner.SetDummyItemTime(2);

            // Make the owner turn in the direction of the blade.
            Owner.ChangeDir(Direction);
        }
        public void CreateProjectiles()
        {
            // Create the slash.
            if (Time == (int)(CarianSlicer.SwingTime * (SwingCompletionRatio + 0.15f)))
                SoundEngine.PlaySound(CarianSlicer.SwingSound, Projectile.Center);
        }
        public void CreateSlashSparkleDust()
        {
            Vector2 initialDirection = InitialRotation.ToRotationVector2();
            Vector2 bladeEnd = Projectile.Center + (GetSwingOffsetAngle(SwingCompletion) * Direction + InitialRotation).ToRotationVector2() * Main.rand.NextFloat(8f, 66f) + initialDirection * 76f;

            int dustID = Main.rand.NextBool() ? 267 : 264;
            Dust magic = Dust.NewDustPerfect(bladeEnd, dustID, Vector2.Zero);
            magic.color = Color.Lerp(CarianSlicer.SwordColor1, CarianSlicer.SwordColor2, Main.rand.NextFloat());
            magic.color = Color.Lerp(magic.color, Color.LightBlue, (float)Math.Pow(Main.rand.NextFloat(), 1.63));
            magic.fadeIn = Main.rand.NextFloat(1f, 2f);
            magic.scale = 0.4f;
            magic.velocity = initialDirection * Main.rand.NextFloat(0.5f, 15f);
            magic.noLight = true;
            magic.noGravity = true;
        }
        #endregion AI and Behaviors

        #region Drawing

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            // Draw the blade.
            DrawBlade(lightColor);

            return false;
        }
        public void DrawBlade(Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * Vector2.UnitY;
            if (Projectile.spriteDirection == -1)
                origin.X += texture.Width;

            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, direction, 0f);
        }
        #endregion Drawing

        #region Hit Effects and Collision
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // TODO: fix collision not syncing with the blade;
            float point = 0f;
            Vector2 direction = (InitialRotation + GetSwingOffsetAngle(SwingCompletion)).ToRotationVector2() * new Vector2(Projectile.spriteDirection, 1f);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + direction * Projectile.height * Projectile.scale, Projectile.width * 0.25f, ref point);
        }
        #endregion Hit Effects and Collision
    }
}