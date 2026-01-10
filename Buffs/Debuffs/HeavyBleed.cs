using Terraria.ModLoader;
using Terraria;

namespace MogMod.Buffs.Debuffs
{
    public class HeavyBleed : ModBuff
    {
        public int tickTimer60 = 0;
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex) //TODO: Fix this it's very broken <-- try the slop they do on soma prime in calamity mod
        {
            //var hitInfoHBleed = new NPC.HitInfo //Hit info used in otherNPC.StrikeNPC(hitInfo)
            //{
            //    Damage = Convert.ToInt32(npc.lifeMax * .0025),
            //    Knockback = 0,
            //    HitDirection = 0,
            //    Crit = false,
            //    DamageType = DamageClass.Default
            //};
            //tickTimer60++;
            //if (tickTimer60 >= 60) //Ok so something in here doesn't work properly, not sure what but I'm giving up for now.
            //{
            //    npc.StrikeNPC(hitInfoHBleed);
            //    tickTimer60 = 0;
            //}
        }
    }
}