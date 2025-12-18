using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.NPCs.Global
{
    // code does nothing for now
    // change to apply buff correctly later

    public class EyeOfSkadiNPCDebuff : GlobalNPC
    {
        public bool skadiApplied = false;

        public override bool InstancePerEntity => true;

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (skadiApplied)
            {
                npc.lifeRegen -= 100;
                npc.defense -= 20;
                npc.damage -= 20;
            }
        }
        public override void ResetEffects(NPC npc)
        {
            skadiApplied = false;
        }
    }
}
