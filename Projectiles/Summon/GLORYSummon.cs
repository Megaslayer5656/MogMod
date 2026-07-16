using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Summon
{
    //52x42
    // TODO: sprite the minion and give it custom ai
    public class GLORYSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetDefaults()
        {
            //Projectile.CloneDefaults(ProjectileID.);
            //AIType = ProjectileID.;
        }
    }
}
