using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class MosinLPSProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        private const int Lifetime = 1800;
        private const int NoDrawing = 2;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;

            Projectile.light = .5f;
            Projectile.timeLeft = Lifetime;
            Projectile.penetrate = 2;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;

            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;
        }
        public override void OnSpawn(IEntitySource source) => Projectile.extraUpdates = Main.zenithWorld ? 10 : 4;
        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft >= Lifetime - NoDrawing * Projectile.MaxUpdates)
                return false;
            MogModUtils.DrawAfterimagesFromEdge(Projectile, 0, lightColor, null);
            return false;
        }
    }
}