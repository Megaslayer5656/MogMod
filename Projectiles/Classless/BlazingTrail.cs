using Microsoft.Xna.Framework;
using MogMod.Common.Graphics;
using MogMod.Items.Accessories.NeutralItems.Aspects;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Classless
{
    public class BlazingTrail : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Classless";
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 80;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.DamageType = DamageClass.Default;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }
        public override void AI()
        {
            if (Projectile.active)
            {
                Projectile.damage = (int)MathF.Max(Owner.HeldItem.damage * BlazingAspect.TrailDamageMult, 1);
                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 playerPosition = Owner.Center + Vector2.UnitY * Owner.gfxOffY;
                    Vector2 orbAttemptedVelocity = Vector2.Zero.MoveTowards(playerPosition - Projectile.Center, 9999f);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, orbAttemptedVelocity, 1f);
                    Projectile.netUpdate = true;

                    var source = Projectile.GetSource_FromThis();
                    var type = ModContent.ProjectileType<BlazingTrailHitbox>();
                    Projectile.NewProjectile(source, Owner.Center, Vector2.Zero, type, Projectile.damage, 0f, Projectile.owner); //, ai2: 6f);
                }
                Projectile.timeLeft = 2;
            }

            if (Owner.dead || !Owner.MogMod().wearingBlazing) Projectile.active = false;
        }
        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.active || Owner.dead || !Owner.MogMod().wearingBlazing || !Owner.MogMod().blazingVisual) return false;
            TrailDrawer trailDrawer = default;
            string type = "MogMod:CometLaserRGB";
            string type2 = "MogMod:FlameLashRGB";
            Color innerColor = new(255, 183, 107);
            Color outerColor = Color.OrangeRed;
            float width = 1f;
            float length = 30f;
            trailDrawer.Draw(Projectile, type, innerColor, outerColor, width, length, length + 8f);
            trailDrawer.Draw(Projectile, type2, innerColor, outerColor, width, length, length + 8f);
            return false;
        }
    }
}