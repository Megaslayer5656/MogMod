using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles
{
    public class MBoulder2 : ModProjectile
    {
        public override void SetDefaults()
        {
            AIType = ProjectileID.MiniBoulder;
            Projectile.CloneDefaults(ProjectileID.MiniBoulder);
        }
    }
}