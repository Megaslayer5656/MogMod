using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Projectiles.BaseProjectiles
{
    /// <summary>
    /// An abstract class dedicated to holdouts.<br/>
    /// Any AI modification MUST be set in <see cref="HoldoutAI"/>, as this class overrides <see cref="AI"/>.<br/>
    /// </summary>
    public abstract class BaseHoldoutProjectile : ModProjectile
    {
        /// <summary>
        /// The player holding the holdout.
        /// </summary>
        public virtual Player Owner => Main.player[Projectile.owner];

        // mostly lifted from fargos, helps with multiplayer syncing
        private int syncTimer;
        private Vector2 mousePos;
        /// <summary>
        /// How fast the holdout should turn.<br/>
        /// Defaults to 0.12f <br/>
        /// </summary>
        public virtual float TurnSpeed => 0.12f;
        /// <summary>
        /// The offset in the holdouts spawn position.<br/>
        /// Defaults to 0f <br/>
        /// </summary>
        public virtual float HoldoutOffset => 0f;
        /// <summary>
        /// The offset in the holdouts rotation.<br/>
        /// Defaults to 0f <br/>
        /// </summary>
        public virtual float RotationOffset => 0f;
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
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(RotationOffset) * Projectile.spriteDirection;

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
            //Projectile.spriteDirection = Projectile.direction * (int)Owner.gravDir;
            Projectile.rotation -= extrarotate;

            Projectile.velocity = Vector2.Lerp(Vector2.Normalize(Projectile.velocity),
                Vector2.Normalize(mousePos - Owner.MountedCenter), TurnSpeed); //slowly move towards direction of cursor
            Projectile.velocity.Normalize();

            if (Projectile.owner == Main.myPlayer)
            {
                mousePos = Main.MouseWorld;

                if (++syncTimer > 20)
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
        /// <summary>
        /// Where any actual AI should go.
        /// </summary>
        public abstract void HoldoutAI();
    }
}