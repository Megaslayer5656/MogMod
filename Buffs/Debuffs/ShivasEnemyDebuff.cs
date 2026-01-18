using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Buffs.Debuffs
{
    public class ShivasEnemyDebuff : ModBuff
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
            if (buffIndex >= 0)
            {
                npc.defense -= Convert.ToInt32(npc.defense * .15f);
                npc.velocity.X *= .99f;
                npc.velocity.Y *= .99f;
                if (Main.rand.NextBool(20))
                {
                    int shivasEnemyDust = Dust.NewDust(npc.Center, 1, 1, DustID.SnowSpray, 0, 0, 0, default, 1f);
                }
            }
        }
    }
}
