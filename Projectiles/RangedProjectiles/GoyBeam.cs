using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Projectiles.BaseProjectiles;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class GoyBeam : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override string Texture => "MogMod/Projectiles/MagicProjectiles/KhandaBeam";
        public override float MaxScale => 1.5f;
        public override float MaxLaserLength => 700f;
        public override float Lifetime => 10f;
        public override Color LightCastColor => Color.LightBlue;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/EnemyProjectiles/Boss/VonLaserStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/EnemyProjectiles/Boss/VonLaserMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/EnemyProjectiles/Boss/VonLaserEnd", AssetRequestMode.ImmediateLoad).Value;
        public int TargetIndex = -1;
        public bool initialized = false;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)Lifetime;
        }
        public override void ExtraBehavior()
        {
            float dustLoopcheck = 16f;
            int dustIncr = 0;
            while (dustIncr < dustLoopcheck)
            {
                Vector2 dustRotate = Vector2.UnitX * 0f;
                dustRotate += -Vector2.UnitY.RotatedBy((double)((float)dustIncr * (6.28318548f / dustLoopcheck)), default) * new Vector2(1f, 4f);
                dustRotate = dustRotate.RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int goyim = Dust.NewDust(Projectile.Center, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, Color.Blue, 1f);
                Main.dust[goyim].scale = 1.5f;
                Main.dust[goyim].noGravity = true;
                Main.dust[goyim].position = Projectile.Center + dustRotate;
                Main.dust[goyim].velocity = Projectile.velocity * 0f + dustRotate.SafeNormalize(Vector2.UnitY) * 1f;
                dustIncr++;
            }
        }
        public override void DetermineScale() => Projectile.scale = Projectile.timeLeft / Lifetime * MaxScale;
        public override bool PreDraw(ref Color lightColor)
        {
            DrawBeamWithColor(Color.Lerp(Color.Blue, Color.Transparent, 0.25f), Projectile.scale);
            DrawBeamWithColor(Color.Lerp(Color.LightBlue * 1.1f, Color.Transparent, 0.25f), Projectile.scale * 0.4f);
            return false;
        }
        public override void OnSpawn(IEntitySource source) => SoundEngine.PlaySound(SoundID.Coins, Projectile.position);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Midas, 600);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Midas, 600);
    }
}