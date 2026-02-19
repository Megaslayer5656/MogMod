using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.MagicProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Projectiles.BaseProjectiles
{
    public partial class MogModGlobalProjectileBleed : GlobalProjectile
    {
        public int bloodDamage;

        public override void SetDefaults(Projectile entity)
        {
            if (entity.type == ModContent.ProjectileType<BloodMagicProjectile>())
            {
                bloodDamage = 15;
            }
            else
            {
                bloodDamage = 0;
            }
        }

        public override bool InstancePerEntity => true;
    }
}
