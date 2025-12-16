using Steamworks;
using System;
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
            EyeOfSkadiNPCDebuff modNPC = npc.ModNPC.

            // fix eye of skadi
            npc.lifeRegen -= 100;
            defenseDown npc. += 200
            npc.defense -= 20;
        }
        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            return true;
        }
    }
    public class EyeOfSkadiNPCDebuff : ModNPC
    {
        public bool skadiApplied = false;
        public override void ResetEffects()
        {
            skadiApplied = false;
        }
    }
}