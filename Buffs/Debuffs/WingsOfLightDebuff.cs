using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Buffs.Debuffs
{
    public class WingsOfLightDebuff : ModBuff
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
            player.MogMod().freezingDebuff = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.MogMod().freezingDebuff < npc.buffTime[buffIndex])
                npc.MogMod().freezingDebuff = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), player.width + 4, player.height + 4, Main.rand.NextBool(3) ? 233 : 246, player.velocity.X * 0.04f, player.velocity.Y * 0.04f, 100, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.65f;
                Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.03f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 0.3f;
                }
            }
        }
        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(npc.position - new Vector2(2f), npc.width + 4, npc.height + 4, Main.rand.NextBool(3) ? 233 : 246, npc.velocity.X * 0.04f, npc.velocity.Y * 0.04f, 100, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.65f;
                Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.03f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 0.3f;
                }
            }
        }
    }
}
