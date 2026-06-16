using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using Terraria;
using Terraria.ModLoader;
namespace MogMod.Buffs.PotionBuffs
{
    public class KrakenShellBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.krakenBuff = true;
            if (Main.rand.NextBool(3))
            {
                Vector2 dustCorner = player.position - 2f * Vector2.One;
                Vector2 dustVel = player.velocity + new Vector2(0f, Main.rand.NextFloat(-5f, -1f));
                int dust = Dust.NewDust(dustCorner, player.width + 4, player.height + 4, Main.rand.NextBool(3) ? 99 : 217, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 1.4f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.75f;
                Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.85f;
                Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1.5f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 0.2f;
                }
            }
        }
    }
}