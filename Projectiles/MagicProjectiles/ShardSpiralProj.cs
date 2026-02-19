using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class ShardSpiralProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Vector2 value7 = new Vector2(5f, 10f);
            Projectile.ai[1] += 1f;

            for (int dust = 0; dust < 2; dust++)
            {
                Vector2 dustPosOffset = Vector2.UnitX * -12f;
                dustPosOffset = -Vector2.UnitY.RotatedBy((double)(Projectile.ai[1] * 0.1308997f + (float)dust * 3.14159274f), default) * value7 - Projectile.rotation.ToRotationVector2() * 10f;
                int exo = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Terragrim, Projectile.velocity.X * 1.5f, Projectile.velocity.Y * 1.5f);
                Main.dust[exo].scale = 1.25f;
                Main.dust[exo].noGravity = true;
                Main.dust[exo].position = Projectile.Center + dustPosOffset;
                Main.dust[exo].velocity = Projectile.velocity;
            }
            MogModUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 500f, 4f, 50f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Terragrim, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}