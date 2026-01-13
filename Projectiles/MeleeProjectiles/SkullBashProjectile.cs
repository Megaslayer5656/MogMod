using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class SkullBashProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public static readonly SoundStyle bashProc = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/SkullBash")
        {
            Volume = 1.3f,
            PitchVariance = .2f
        };
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.extraUpdates = 100;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.alpha = 0;

        }
        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            Vector2 projPos = Projectile.position;
            projPos -= Projectile.velocity;
            int suvass = Dust.NewDust(projPos, 1, 1, DustID.Blood, 0f, 0f, 0, Color.Red, 1f);
            Main.dust[suvass].alpha = 200;
            Main.dust[suvass].velocity *= 1.4f;
            Main.dust[suvass].scale += Main.rand.NextFloat();
        }
        public override void OnSpawn(IEntitySource source)
        {
        SoundEngine.PlaySound(bashProc, Projectile.Center);
        }
    }
}
