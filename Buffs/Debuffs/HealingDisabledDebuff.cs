using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Buffs.Debuffs
{
    public class HealingDisabledDebuff : ModBuff
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
            player.MogMod().healingDisabledDebuff = true;
        }
        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), player.width + 4, player.height + 4, 74, player.velocity.X * 1.4f, player.velocity.Y * 1.4f, 100, default, 1.6f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.85f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= .95f;
                }
            }
        }
    }
}