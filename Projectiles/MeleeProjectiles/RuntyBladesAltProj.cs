using Microsoft.Xna.Framework;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class RuntyBladesAltProj : BaseShortswordProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override float FadeInDuration => 8f;
        public override float FadeOutDuration => 0f;
        public override float TotalDuration => 12f;
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(15);
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void SetVisualOffsets()
        {
            const int HalfSpriteWidth = 32 / 2;
            const int HalfSpriteHeight = 32 / 2;

            int HalfProjWidth = Projectile.width / 2;
            int HalfProjHeight = Projectile.height / 2;

            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
            DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);
        }
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12), DustID.Tungsten);
                dust.scale = Main.rand.NextFloat(0.15f, 0.6f);
                dust.noGravity = true;
                dust.velocity = -Projectile.velocity * 0.5f;
            }

            float armPointingDirection = (Main.MouseWorld - Owner.MountedCenter).ToRotation();
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armPointingDirection - MathHelper.PiOver2);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, 0);
            Owner.heldProj = Projectile.whoAmI;

            Projectile.Center = Projectile.Center + new Vector2(0, -6f);
        }
    }
}