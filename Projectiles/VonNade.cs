using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace MogMod.Projectiles
{
    public class VonNade : ModProjectile
    {
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
