using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace MogMod.Buffs.Debuffs
{
    public class GreenTracerDebuff : ModBuff
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
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust dust2 = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.TerraBlade);
                    dust2.scale = Main.rand.NextFloat(0.6f, 0.8f);
                }
            }
        }
    }
}