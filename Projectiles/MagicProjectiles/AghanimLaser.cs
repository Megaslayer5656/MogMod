using MogMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using MogMod.Utilities;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class AghanimLaser : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
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
            MogModUtils.HomeInOnNPC(Projectile, true, 1500f, 1f, 20f);
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
            // number at the very end makes it opacke
            DrawBeamWithColor(new Color(128, 0, 255, 0), Projectile.scale);
            DrawBeamWithColor(Color.Lerp(new Color(255, 0, 255, 0), Color.Transparent, 0.35f), Projectile.scale * 0.6f);
            DrawBeamWithColor(Color.Lerp(new Color(128, 0, 255, 0), Color.Transparent, 0.35f), Projectile.scale * 0.6f);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 600);
            target.AddBuff(BuffID.StardustMinionBleed, 600);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Daybreak, 600);
            target.AddBuff(BuffID.StardustMinionBleed, 600);
        }
    }
}