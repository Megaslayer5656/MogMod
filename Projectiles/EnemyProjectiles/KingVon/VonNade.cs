using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace MogMod.Projectiles.EnemyProjectiles.KingVon
{
    public class VonNade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.BossProjectiles";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Grenade);
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.damage = 200;
            AIType = ProjectileID.Grenade;
        }
    }
}
