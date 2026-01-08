using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace MogMod.Buffs.Debuffs
{
    public class ShockState : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            int shockDust = Dust.NewDust(npc.Center, 1, 1, DustID.Electric, 0, 0, 0, default, .5f); //Shock state effect on target
        }
    }
}
