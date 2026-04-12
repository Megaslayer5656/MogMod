using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class WaveProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 8;

            Projectile.DamageType = DamageClass.Ranged;

            Projectile.timeLeft = 300;

            Projectile.friendly = true;

            AIType = ProjectileID.Bullet;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
        }
        public override void AI()
        {
            Projectile.ai[1]++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.ai[1] <= 10)
                Projectile.velocity.Y = -6f;
            if (Projectile.ai[1] >= 11)
                Projectile.velocity.Y = 6f;
            if (Projectile.ai[1] >= 20)
                Projectile.ai[1] = 0;

            Dust dust = Dust.NewDustPerfect(Projectile.Center, 264, -Projectile.velocity * 0.05f);
            dust.noGravity = true;
            dust.scale = 1.2f;
            dust.color = Color.LightBlue;
            dust.fadeIn = 1.8f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Main.rand.NextBool(2))
                Projectile.ai[1] = 10;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
