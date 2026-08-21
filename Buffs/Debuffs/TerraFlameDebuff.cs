using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Buffs.Debuffs
{
    public class TerraFlameDebuff : ModBuff
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
            player.MogMod().terraFlameDebuff = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.MogMod().terraFlameDebuff = true;
        }
        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), player.width + 4, player.height + 4, Main.rand.NextBool(3) ? DustID.FireworksRGB : DustID.RainbowTorch, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, Utils.SelectRandom(Main.rand, new Color[] { new(247, 255, 120), new(71, 255, 236) }), 1.4f);
            Main.dust[dust].noGravity = true;
            float rotation = Main.rand.NextFloat(0.3f, 1f);
            Main.dust[dust].velocity = new Vector2(0, -2).RotatedByRandom(rotation * 0.3f) * (Main.rand.NextFloat(1f, 2.9f) - rotation);
            Main.dust[dust].scale = Main.rand.NextFloat(1.2f, 1.8f) * (60 * 0.015f);
        }
        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            int dust = Dust.NewDust(npc.position - new Vector2(2f), npc.width + 4, npc.height + 4, Main.rand.NextBool(3) ? DustID.FireworksRGB : DustID.RainbowTorch, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, Utils.SelectRandom(Main.rand, new Color[] { new(247, 255, 120), new(71, 255, 236),  }), 1.4f);
            Main.dust[dust].noGravity = true;
            float rotation = Main.rand.NextFloat(0.3f, 1f);
            Main.dust[dust].velocity = new Vector2(0, -2).RotatedByRandom(rotation * 0.3f) * (Main.rand.NextFloat(1f, 2.9f) - rotation);
            Main.dust[dust].scale = Main.rand.NextFloat(1.2f, 1.8f) * (60 * 0.015f);
        }
    }
}