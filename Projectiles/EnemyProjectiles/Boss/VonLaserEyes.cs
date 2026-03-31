using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.NPCs.Bosses;
using MogMod.Projectiles.BaseProjectiles;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.EnemyProjectiles.Boss
{
    public class VonLaserEyes : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.BossProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override float MaxScale => KingVon.laserScale;
        public override float MaxLaserLength => KingVon.laserLength;
        public override float Lifetime => KingVon.laserLifetime;
        public override Color LightCastColor => Color.Red;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/InvisibleProj", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/PhylacteryMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/PhylacteryEnd", AssetRequestMode.ImmediateLoad).Value;
        public int TargetIndex = -1;
        public bool initialized = false;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)Lifetime;
        }
        public override void DetermineScale() => Projectile.scale = Projectile.timeLeft / Lifetime * MaxScale;
        public override bool PreDraw(ref Color lightColor)
        {
            DrawBeamWithColor(Color.Lerp(Color.Red, Color.Transparent, 0.25f), Projectile.scale);
            DrawBeamWithColor(Color.Lerp(Color.IndianRed * 1.1f, Color.Transparent, 0.25f), Projectile.scale * 0.4f);
            return false;
        }
    }
}