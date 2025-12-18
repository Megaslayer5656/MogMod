using MogMod.NPCs.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Buffs
{
    public class EyeOfSkadiDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = false;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen -= 250;
            // change to make it apply a buff instead of constant hp regen reduction
            // npc.GetGlobalNPC<EyeOfSkadiNPCDebuff>().skadiApplied = true;
        }
    }
}
