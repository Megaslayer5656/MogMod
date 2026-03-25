using Terraria.ModLoader;

namespace MogMod.NPCs.Global
{
    public class ProjectileModificationGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int timesHitByModifiedProjectiles;
    }
}
