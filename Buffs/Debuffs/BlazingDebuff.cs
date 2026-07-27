using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Buffs.Debuffs
{
    public class BlazingDebuff : ModBuff
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
            player.MogMod().blazingDebuff = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.MogMod().blazingDebuff = true;
        }
        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            int hot = Dust.NewDust(drawInfo.Position - new Vector2(2f), player.width + 4, player.height + 4, DustID.Flare, player.velocity.X * 1.15f, player.velocity.Y * 1.15f, 100, default, 1.4f);
            Main.dust[hot].noGravity = false;
            Main.dust[hot].velocity *= 1.15f;
            Main.dust[hot].scale *= .95f;
            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), player.width + 4, player.height + 4, DustID.Lava, player.velocity.X * 1.15f, player.velocity.Y * 1.15f, 100, default, 0.8f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.35f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= .95f;
                }
            }
        }
        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            int hot = Dust.NewDust(npc.position - new Vector2(2f), npc.width + 4, npc.height + 4, DustID.Flare, npc.velocity.X * 1.15f, npc.velocity.Y * 1.15f, 100, default, 1.4f);
            Main.dust[hot].noGravity = false;
            Main.dust[hot].velocity *= 1.15f;
            Main.dust[hot].scale *= .95f;
            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(npc.position - new Vector2(2f), npc.width + 4, npc.height + 4, DustID.Lava, npc.velocity.X * 1.15f, npc.velocity.Y * 1.15f, 100, default, 0.8f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.35f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 0.95f;
                }
            }
        }
    }
}