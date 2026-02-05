using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Buffs.Debuffs
{
    public class Bleed1 : ModBuff
    {
        public int hits = 1;
        public NPC.HitInfo hitInfo;

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            hits++;
            return true;
        }

        public override void Update(NPC npc, ref int buffIndex) //TODO: Add vfx and a sound to the bleed proc
        {
            if (hits >= 7)
            {
                if (npc.lifeMax <= 25000)
                {
                    hitInfo = new NPC.HitInfo
                    {
                        Damage = Convert.ToInt32(npc.lifeMax * .05),
                        Knockback = 0,
                        HitDirection = 0,
                        Crit = false,
                        DamageType = DamageClass.Generic
                    };
                } else
                {
                    hitInfo = new NPC.HitInfo
                    {
                        Damage = 1250,
                        Knockback = 0,
                        HitDirection = 0,
                        Crit = false,
                        DamageType = DamageClass.Generic
                    };
                }
                npc.StrikeNPC(hitInfo);
                NetMessage.SendStrikeNPC(npc, hitInfo);
                hits = 1;
                npc.DelBuff(buffIndex);
            }
        }
    }
}
