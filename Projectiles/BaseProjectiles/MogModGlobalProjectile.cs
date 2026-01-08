using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace MogMod.Projectiles.BaseProjectiles
{
    public partial class MogModGlobalProjectile : GlobalProjectile
    {
        // exists for projectile utils hopefully
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }
        // Amount of extra updates that are set in SetDefaults.
        public int defExtraUpdates = -1;
    }
}
