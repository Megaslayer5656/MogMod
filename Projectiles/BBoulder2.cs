using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles
{
    public class BBoulder2 : ModProjectile
    {
        public override void SetDefaults()
        {
            AIType = ProjectileID.BouncyBoulder;
            Projectile.CloneDefaults(ProjectileID.BouncyBoulder);
            owner = Main.myPlayer;
        }
    }
}
