using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.TheGravitySpells
{
    public class EmptySpellCard : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Magic;
        }
        public override void AI()
        {
            if (Projectile.timeLeft <= 8)
            {
                Projectile.velocity = Vector2.Zero;
                for (int n = 0; n < 6; n++)
                {
                    float swirlRotation = Main.GlobalTimeWrappedHourly * -5.75f + (MathHelper.TwoPi / 6f * n);
                    Vector2 swirlPos = Projectile.Center + Vector2.UnitX.RotatedBy(swirlRotation) * 20f;
                    Vector2 swirlVelocity = Vector2.Normalize(swirlPos - Projectile.Center).RotatedBy(MathHelper.ToRadians(140)) * 2f;
                    Dust swirlDust = Dust.NewDustPerfect(swirlPos, DustID.PlatinumCoin, swirlVelocity * Main.rand.NextFloat(2f, 4f), 0, default, 1.5f);
                    swirlDust.noGravity = true;
                }
            }
        }
        public override bool? CanDamage() => false;
    }
}
