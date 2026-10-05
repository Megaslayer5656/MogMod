using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Common.Graphics;
using MogMod.Items.Weapons.Magic;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using Mono.Cecil;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class KhandaBeam : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/PhylacteryStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/PhylacteryMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/PhylacteryEnd", AssetRequestMode.ImmediateLoad).Value;
        public override float MaxScale => 2.1f;
        public override float MaxLaserLength => 1000f;
        public override float Lifetime => 3600f;
        private Projectile Holdout => Main.projectile[(int)Projectile.ai[1]];
        public override Color LaserOverlayColor => Khanda.WeakColor;
        public static Color StrongColor => Khanda.StrongColor;
        public SlotId AudSlot;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 5000;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.netImportant = true;
        }
        public override void AttachToSomething()
        {
            if (Owner.CantUseHoldout() || Holdout.ai[2] == 5f)
            {
                if (Projectile.timeLeft > 2) Projectile.timeLeft = 2;
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();
            }
            if (Owner.active && !Owner.dead && Holdout.ai[2] != 5f) Projectile.Center = Holdout.Center + Vector2.Normalize(Projectile.velocity * 20f);
        }
        public override void UpdateLaserMotion()
        {
            if (Holdout.velocity != Projectile.velocity) Projectile.netUpdate = true;
            Projectile.velocity = Holdout.velocity;
        }
        public override void ExtraBehavior()
        {
            Projectile.rotation = Holdout.velocity.ToRotation();
            if (Owner.channel && Time >= 30f)
            {
                Projectile.timeLeft++;
                Time--;
            }
            if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound) && ChargeSound.IsPlaying)
            {
                ChargeSound.Position = Projectile.Center;
                ChargeSound.Pitch = 0.4f;
                ChargeSound.Volume = 0.75f * 100;
            }
            else if (Time < 30f) AudSlot = SoundEngine.PlaySound(SoundID.DD2_EtherianPortalIdleLoop with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
        }
        public override void DetermineScale() => Projectile.scale = MathHelper.Lerp(0f, 1f, Time / 30f) * MaxScale;
        public override float DetermineLaserLength() => DetermineLaserLength_CollideWithTiles();
        public override bool ShouldUpdatePosition() => false;
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 laserEnd = Projectile.Center + Projectile.velocity * LaserLength;
            Utils.PlotTileLine(Projectile.Center, laserEnd, Projectile.width + 16, DelegateMethods.CutTiles);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero) return false;

            // Draw the actual laser
            Vector2 laserEnd = Projectile.Center + Projectile.velocity * LaserLength;
            int length = 10;
            Vector2[] drawPoints = new Vector2[length];
            float[] rotPoints = new float[length];
            TrailDrawer trailDrawer = default;
            for (int i = 0; i < length; i++)
            {
                Color innerDrawColor = Projectile.GetAlpha(StrongColor);
                Color outerDrawColor = Projectile.GetAlpha(LaserOverlayColor);
                drawPoints[i] = Vector2.Lerp(Projectile.Center, laserEnd, i / (float)(drawPoints.Length - 1f));
                rotPoints[i] = Projectile.rotation - MathHelper.Pi;
                //Main.NewText($"{Projectile.rotation}, {rotPoints[i]}, {drawPoints[i]}");
                trailDrawer.Draw(Projectile, "MogMod:MagicMissileRGB", outerDrawColor, innerDrawColor, Math.Abs(0.5f - MaxScale), 30f, 44f, drawPoints, rotPoints);
            }
            return false;
        }
    }
}