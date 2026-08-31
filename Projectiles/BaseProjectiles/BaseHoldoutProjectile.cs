using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Utilities;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace MogMod.Projectiles.BaseProjectiles
{
    /// <summary>
    /// An abstract class dedicated to holdouts.<br/>
    /// Any AI modification MUST be set in <see cref="HoldoutAI"/>, as this class overrides <see cref="AI"/>.<br/>
    /// </summary>
    public abstract class BaseHoldoutProjectile : ModProjectile
    {
        #region Fields
        /// <summary>
        /// The player holding the holdout.
        /// </summary>
        public virtual Player Owner => Main.player[Projectile.owner];
        private int syncTimer;
        private Vector2 mousePos;
        #endregion

        #region Overridable Fields
        /// <summary>
        /// How fast the holdout should turn.<br/>
        /// Defaults to 0.12f <br/>
        /// </summary>
        public virtual float TurnSpeed => 0.12f;
        /// <summary>
        /// The offset applied to the holdouts spawn position.<br/>
        /// Defaults to 0f <br/>
        /// </summary>
        public virtual float HoldoutOffset => 0f;
        /// <summary>
        /// The offset in degrees applied to the holdouts rotation.<br/>
        /// The value is automatically converted to radians.<br/>
        /// Defaults to 0f <br/>
        /// </summary>
        public virtual float RotationOffset => 0f;
        /// <summary>
        /// What style of handling the holdout should use when tracking it's target.<br/>
        /// Defaults to <see cref="HoldoutStyle.Rigid"/>
        /// </summary>
        public virtual int HoldoutHandling => HoldoutStyle.Rigid;
        #endregion

        #region Properties
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(mousePos.X);
            writer.Write(mousePos.Y);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Vector2 buffer;
            buffer.X = reader.ReadSingle();
            buffer.Y = reader.ReadSingle();
            if (Projectile.owner != Main.myPlayer)
            {
                mousePos = buffer;
            }
        }
        public override void AI()
        {
            if (Owner.dead || !Owner.active)
            {
                Projectile.Kill();
                return;
            }

            UpdatePlayerVisuals();
            HoldoutAI();
        }
        private void UpdatePlayerVisuals()
        {
            Vector2 center = Owner.MountedCenter;

            Projectile.Center = center;
            if (HoldoutHandling == HoldoutStyle.Rigid) Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(RotationOffset) * Projectile.spriteDirection; // more rigid and strict
            else if (HoldoutHandling == HoldoutStyle.Floaty) Projectile.rotation = Projectile.AngleTo(mousePos) + MathHelper.ToRadians(RotationOffset) * Projectile.spriteDirection; // more wobbly and bouncy

            float extrarotate = Owner.gravDir < 0 ? MathHelper.Pi : 0;
            float itemrotate = Projectile.direction < 0 ? MathHelper.Pi : 0;
            Owner.itemRotation = Projectile.velocity.ToRotation() + itemrotate;
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 10;
            Owner.itemAnimation = 10;

            Vector2 HoldOffset = new Vector2(HoldoutOffset, 0).RotatedBy(MathHelper.WrapAngle(Projectile.velocity.ToRotation()));

            Projectile.Center += HoldOffset;
            Projectile.spriteDirection = Projectile.direction * (int)Owner.gravDir;
            Projectile.rotation -= extrarotate;
            if (Projectile.spriteDirection == -1) Projectile.rotation -= extrarotate * 2;

            Projectile.velocity = Vector2.Lerp(Vector2.Normalize(Projectile.velocity), Vector2.Normalize(mousePos - Owner.MountedCenter), TurnSpeed); //slowly move towards direction of cursor
            Projectile.velocity.Normalize();

            if (Projectile.owner == Main.myPlayer)
            {
                mousePos = Main.MouseWorld;

                if (++syncTimer > 2)
                {
                    syncTimer = 0;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Projectile.Center += Projectile.velocity * 20;
                return;
            }
        }
        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            PreDrawBehind(ref lightColor);

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            int direction = Main.player[Projectile.owner].direction;
            Vector2 origin = texture.Size() * 0.5f;
            SpriteEffects flip = direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.EntitySpriteDraw(texture, drawPos, null, lightColor, Projectile.rotation, origin, Projectile.scale, flip);
            return false;
        }
        #endregion

        #region Overridable Properties
        /// <summary>
        /// Where any actual AI should go.
        /// </summary>
        public abstract void HoldoutAI();
        /// <summary>
        /// Use this to draw things behind the projectile. Mostly used for glow drawing.<br/>
        /// Keep in mind this is called immediately in <see cref="PreDraw"/>.<br/>
        /// </summary>
        /// <param name="lightColor">The color of the light at the projectiles center.</param>
        public virtual void PreDrawBehind(ref Color lightColor)
        {

        }
        #endregion
    }
    /// <summary> What type of movement the holdout should use when following it's target.</summary>
    public static partial class HoldoutStyle
    {
        /// <summary> Directly moves towards the target.</summary>
        public static int Rigid = 0;
        /// <summary> Loosely moves towards the target while overextending slightly.</summary>
        public static int Floaty = 1;
    }
}