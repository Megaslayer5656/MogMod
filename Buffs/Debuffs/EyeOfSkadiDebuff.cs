using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Buffs.Debuffs
{
    public class EyeOfSkadiDebuff : ModBuff
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
            player.MogMod().skadiDebuff = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.MogMod().skadiDebuff < npc.buffTime[buffIndex])
                npc.MogMod().skadiDebuff = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), player.width + 4, player.height + 4, Main.rand.NextBool(3) ? 104 : 186, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 1.4f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0.75f;
            Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.75f;
            Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1f;
            if (Main.rand.NextBool(4))
            {
                Main.dust[dust].noGravity = false;
                Main.dust[dust].scale *= 0.2f;
            }
        }
        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            int dust = Dust.NewDust(npc.position - new Vector2(2f), npc.width + 4, npc.height + 4, Main.rand.NextBool(3) ? 104 : 186, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 1.4f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0.75f;
            Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.75f;
            Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1f;
            if (Main.rand.NextBool(4))
            {
                Main.dust[dust].noGravity = false;
                Main.dust[dust].scale *= 0.2f;
            }
        }
    }
}
