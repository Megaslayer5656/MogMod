using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Buffs.Debuffs
{
    public class InfernoDebuff : ModBuff
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
            player.MogMod().infernoDebuff = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.MogMod().infernoDebuff = true;
        }
        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            int hot = Dust.NewDust(drawInfo.Position - new Vector2(2f), player.width + 4, player.height + 4, DustID.Torch, player.velocity.X * 1.4f, player.velocity.Y * 1.4f, 100, default, 2.4f);
            Main.dust[hot].noGravity = false;
            Main.dust[hot].velocity *= 1.85f;
            Main.dust[hot].scale *= .95f;
            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), player.width + 4, player.height + 4, DustID.DesertTorch, player.velocity.X * 1.4f, player.velocity.Y * 1.4f, 100, default, 2.4f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.85f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= .95f;
                }
            }
        }
        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            int hot = Dust.NewDust(npc.position - new Vector2(2f), npc.width + 4, npc.height + 4, DustID.Torch, npc.velocity.X * 1.4f, npc.velocity.Y * 1.4f, 100, default, 2.4f);
            Main.dust[hot].noGravity = false;
            Main.dust[hot].velocity *= 1.85f;
            Main.dust[hot].scale *= .95f;
            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(npc.position - new Vector2(2f), npc.width + 4, npc.height + 4, DustID.DesertTorch, npc.velocity.X * 1.4f, npc.velocity.Y * 1.4f, 100, default, 2.4f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.85f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 0.95f;
                }
            }
        }
    }
}