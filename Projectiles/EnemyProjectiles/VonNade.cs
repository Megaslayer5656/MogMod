using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace MogMod.Projectiles.EnemyProjectiles
{
    public class VonNade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles";
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
