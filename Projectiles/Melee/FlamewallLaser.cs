using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Common.Graphics;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class FlamewallLaser : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        //public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        public override string Texture => "MogMod/Projectiles/MagicProjectiles/KhandaBeam";
        public Player Owner => Main.player[Projectile.owner];
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/PhylacteryStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/PhylacteryMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/PhylacteryEnd", AssetRequestMode.ImmediateLoad).Value;
        public override float MaxScale => 3f;
        public override float MaxLaserLength => 2400f;
        public override float Lifetime => 3600f;
        private Projectile Holdout => Main.projectile[(int)Projectile.ai[1]];
        public static readonly SoundStyle HitSound = SoundID.DD2_FlameburstTowerShot with { Volume = 0.8f };
        public override Color LaserOverlayColor => Flamewall.WeakColor;
        public static Color StrongColor => Flamewall.StrongColor;
        public int HitSoundCooldown = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 5000;
            //ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            //ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9;
            Projectile.hide = true;
            Projectile.netImportant = true;
        }
        public override void AttachToSomething()
        {
            if (Owner.CantUseHoldout())
            {
                if (Projectile.timeLeft > 2) Projectile.timeLeft = 2;
            }
            if (Owner.active && !Owner.dead) Projectile.Center = Holdout.Center - new Vector2(0f, 130f) * Holdout.scale;
        }
        public override void UpdateLaserMotion()
        {
            Vector2 aimVector = (Owner.MogMod().mouseWorld - Holdout.Center).SafeNormalize(Vector2.UnitX);
            aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(Projectile.velocity), 0.94f));

            if (aimVector != Projectile.velocity) Projectile.netUpdate = true;
            Projectile.velocity = aimVector;
            //Projectile.rotation = Projectile.velocity.ToRotation();
            //Projectile.velocity += (Owner.MogMod().mouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX);
        }
        public override void DetermineScale()
        {
            if (Time < 30f) Projectile.scale = MathHelper.Lerp(0f, 1f, Time / 30f) * MaxScale;
            else Projectile.scale = Utils.GetLerpValue(0f, 30f, Projectile.timeLeft, true) * MaxScale;
        }
        public override void ExtraBehavior()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Main.NewText(Projectile.velocity);
            Owner.SetScreenshake(3f);
            if (Owner.channel)
            {
                Projectile.timeLeft++;
                Time--;
            }
            if (HitSoundCooldown > 0) HitSoundCooldown--;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<InfernoDebuff>(), 300);

            if (HitSoundCooldown == 0)
            {
                SoundEngine.PlaySound(HitSound, target.Center);
                HitSoundCooldown = 9;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero) return false;

            // Draw the actual lase
            Vector2 laserEnd = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * LaserLength;
            //Main.NewText($"{laserEnd}");
            Vector2[] drawPoints = new Vector2[10];
            TrailDrawer trailDrawer = default;
            Color innerDrawColor = Projectile.GetAlpha(LaserOverlayColor);
            Color outerDrawColor = Projectile.GetAlpha(StrongColor);
            for (int i = 0; i < drawPoints.Length; i++)
            {
                drawPoints[i] = Vector2.Lerp(Projectile.Center, laserEnd, i / (float)(drawPoints.Length - 1f));
                trailDrawer.Draw(Projectile, "MogMod:FlameLashRGB", outerDrawColor, innerDrawColor, 1.1f, 30f, 44f, drawPoints);
            }
            return base.PreDraw(ref lightColor);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
    }
}