using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Common.Graphics;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class AghanimLaser : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "MogMod/Projectiles/MagicProjectiles/KhandaBeam";
        public override float MaxScale => 1.8f;
        public override float MaxLaserLength => 1500f;
        public override float Lifetime => 570f;
        public override Color LightCastColor => Color.BlueViolet;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/PhylacteryStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/PhylacteryMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/PhylacteryEnd", AssetRequestMode.ImmediateLoad).Value;
        public const float UniversalAngularSpeed = MathHelper.Pi / 600f;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)Lifetime;
        }
        public override void ExtraBehavior()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            MogModUtils.HomeInOnNPC(Projectile, true, 1500f, 1f, 20f);
            //Projectile.alpha = (int)Math.Abs(255 + MathHelper.Lerp(0, 255, -(Projectile.scale / MaxScale)));
            Projectile.alpha = (int)MathHelper.Lerp(255, 0, (Projectile.scale / MaxScale));
            //RotationalSpeed = UniversalAngularSpeed;
            // Generate a burst of bubble-like nebula dust.
            if (!Main.dedServ && Time == 5f)
            {
                int totalBubbles = 3;
                for (int k = 0; k < totalBubbles; k++)
                {
                    Dust nebulaBubble = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowTorch, Projectile.velocity, 0, Color.DarkViolet);
                    nebulaBubble.scale = Main.rand.NextFloat(1.6f, 1.8f);
                    nebulaBubble.noGravity = true;
                }
            }
        }
        public override void DetermineScale() => Projectile.scale = Projectile.timeLeft / Lifetime * MaxScale;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero) return false;

            // Draw the actual laser
            Vector2 laserEnd = Projectile.Center + Projectile.velocity * LaserLength;
            int length = 10;
            Vector2[] drawPoints = new Vector2[length];
            float[] rotPoints = new float[length];
            TrailDrawer trailDrawer = default;
            float scale = MathHelper.Clamp(Utils.GetLerpValue(2f, 0.95f, Projectile.scale / MaxScale), 0.95f, 2f);
            for (int i = 0; i < length; i++)
            {
                Color innerDrawColor = Projectile.GetAlpha(LightCastColor);
                Color outerDrawColor = Projectile.GetAlpha(Color.Orchid);
                drawPoints[i] = Vector2.Lerp(Projectile.position, laserEnd, i / (float)(drawPoints.Length - 1f));
                rotPoints[i] = Projectile.rotation - MathHelper.Pi;
                trailDrawer.Draw(Projectile, "MogMod:MagicMissileRGB", outerDrawColor, innerDrawColor, scale, 60f, 64f, drawPoints, rotPoints);
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AghanimHexDebuff>(), 600);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<AghanimHexDebuff>(), 600);
        }
    }
}