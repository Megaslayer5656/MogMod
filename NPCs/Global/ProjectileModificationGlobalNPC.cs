using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace MogMod.NPCs.Global
{
    public class ProjectileModificationGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int timesHitByModifiedProjectiles;
    }
}
