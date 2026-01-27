using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace MogMod.Buffs.Debuffs
{
    public class FreezingDebuff : ModBuff
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
            int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), player.width + 4, player.height + 4, Main.rand.NextBool(3) ? 185 : 197, player.velocity.X * 0.04f, player.velocity.Y * 0.04f, 100, default, 1.4f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0.05f;
            Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.03f;
            //Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1f;
            if (Main.rand.NextBool(4))
            {
                Main.dust[dust].noGravity = false;
                Main.dust[dust].scale *= 0.2f;
            }
        }
        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            int dust = Dust.NewDust(npc.position - new Vector2(2f), npc.width + 4, npc.height + 4, Main.rand.NextBool(3) ? 185 : 197, npc.velocity.X * 0.04f, npc.velocity.Y * 0.04f, 100, default, 1.4f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0.05f;
            Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.03f;
            //Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1f;
            if (Main.rand.NextBool(4))
            {
                Main.dust[dust].noGravity = false;
                Main.dust[dust].scale *= 0.2f;
            }
        }
    }
}
