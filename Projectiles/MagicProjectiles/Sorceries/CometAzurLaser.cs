using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Classes;
using MogMod.Common.Graphics;
using MogMod.Items.Ammo.SorcerySpells.Legendary;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class CometAzurLaser : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/PhylacteryStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/PhylacteryMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/PhylacteryEnd", AssetRequestMode.ImmediateLoad).Value;
        public override float MaxScale => 4f;
        public override float MaxLaserLength => 1500f;
        public override float Lifetime => 3600f;
        private Projectile Holdout => Main.projectile[(int)Projectile.ai[1]];
        public static Color WeakColor => CometAzur.WeakColor;
        public static Color StrongColor => CometAzur.StrongColor;
        private const int hitCooldown = 1;
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
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = hitCooldown;
            Projectile.netImportant = true;
            Projectile.hide = true;
        }
        public override void AttachToSomething()
        {
            if (Owner.CantUseHoldout() || (Holdout.ai[2] == 5f && !Main.zenithWorld))
            {
                if (Projectile.timeLeft > 2) Projectile.timeLeft = 2;
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();
                Projectile.netUpdate = true;
            }
            if (Owner.active && !Owner.dead && (Holdout.ai[2] != 5f && !Main.zenithWorld)) Projectile.Center = Holdout.Center + Projectile.velocity * 30f;
            if (Owner.active && !Owner.dead && Main.zenithWorld) Projectile.Center = Owner.MountedCenter;
        }
        public override void UpdateLaserMotion()
        {
            if (Holdout.velocity != Projectile.velocity) Projectile.netUpdate = true;
            if (!Main.zenithWorld) Projectile.velocity = Holdout.velocity;
            else Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(1f));
        }
        public override void ExtraBehavior()
        {
            Owner.PickAmmo(Owner.HeldItem, out _, out float speed, out int damage, out float knockback, out _, true);
            Projectile.damage = damage;
            Projectile.originalDamage = Projectile.damage;
            Projectile.rotation = (Main.zenithWorld ? Projectile.velocity.ToRotation() : Holdout.velocity.ToRotation());
            if (Owner.channel && Time >= 30f)
            {
                Projectile.timeLeft++;
                Time--;
            }
            if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound) && ChargeSound.IsPlaying)
            {
                ChargeSound.Position = Projectile.Center;
                ChargeSound.Pitch = 0.2f;
                ChargeSound.Volume = 1f * 100;
            }
            else if (Time < 30f) AudSlot = SoundEngine.PlaySound(SoundID.DD2_EtherianPortalIdleLoop with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
        }
        public override void DetermineScale() => Projectile.scale = MathHelper.Lerp(0f, 1f, Time / 30f) * MaxScale;
        public override bool ShouldUpdatePosition() => false;
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 laserEnd = Projectile.Center + Projectile.velocity * LaserLength;
            Utils.PlotTileLine(Projectile.Center, laserEnd, Projectile.width + 16, DelegateMethods.CutTiles);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overWiresUI.Add(index);
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero) return false;

            // Draw the actual laser
            Vector2 laserEnd = (Projectile.Center - Vector2.Normalize(Projectile.velocity * 20f)) + Projectile.velocity * LaserLength;
            int length = 10;
            Vector2[] drawPoints = new Vector2[length];
            float[] rotPoints = new float[length];
            TrailDrawer trailDrawer = default;
            for (int i = 0; i < length; i++)
            {
                Color innerDrawColor = Projectile.GetAlpha(WeakColor);
                Color outerDrawColor = Projectile.GetAlpha(StrongColor);
                drawPoints[i] = Vector2.Lerp(Projectile.position, laserEnd, i / (float)(drawPoints.Length - 1f));
                rotPoints[i] = Projectile.rotation - MathHelper.Pi;
                float scale = 0.75f;
                trailDrawer.Draw(Projectile, "MogMod:CometLaserRGB", outerDrawColor, innerDrawColor, scale, 30f, 44f, drawPoints, rotPoints);
            }
            // draw glow effect
            Vector2 drawPosition = (Projectile.Center - Vector2.Normalize(Projectile.velocity * 20f) + Projectile.velocity) - Main.screenPosition;
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Assets/Textures/GlowParticle").Value;
            for (int i = 0; i < 5; i++)
            {
                float increment = MathHelper.Lerp(1f, 1.5f, MathF.Sin(Main.GlobalTimeWrappedHourly * 2) * 0.5f + 0.5f);
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, StrongColor * 0.85f, Projectile.rotation, bloomTex.Size() * 0.5f, 0.5f * increment, SpriteEffects.None);
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, WeakColor * 0.8f, Projectile.rotation, bloomTex.Size() * 0.5f, 0.525f * increment, SpriteEffects.None);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}