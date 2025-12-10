using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles
{
        public class Boulder2 : ModProjectile
    {
        public override void SetDefaults()
        {
            AIType = ProjectileID.Boulder;
            Projectile.CloneDefaults(ProjectileID.Boulder);
            owner = Main.myPlayer;
        }
    }
}
