using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Buffs.Debuffs
{
    public class DivineMightDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.MogMod().divineDebuff = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.MogMod().divineDebuff < npc.buffTime[buffIndex])
                npc.MogMod().divineDebuff = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust dust2 = Dust.NewDustDirect(player.position, player.width, player.height, DustID.ShimmerSpark);
                    dust2.scale = Main.rand.NextFloat(0.5f, 0.7f);
                }
            }
        }
        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust dust2 = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.ShimmerSpark);
                    dust2.scale = Main.rand.NextFloat(0.5f, 0.7f);
                }
            }
        }
    }
}