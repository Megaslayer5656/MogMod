using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Buffs.PotionBuffs
{
    public class SatanicBuff : ModBuff
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
            mogPlayer.satanicBuff = true;
            if (Main.rand.NextBool(3))
            {
                Vector2 dustCorner = player.position - 2f * Vector2.One;
                Vector2 dustVel = player.velocity + new Vector2(0f, Main.rand.NextFloat(-5f, -1f));
                int d = Dust.NewDust(dustCorner, player.width, player.height, DustID.LifeDrain, dustVel.X, dustVel.Y);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity.Y -= 1.5f;
                Main.dust[d].scale = 0.8f;
                Main.dust[d].fadeIn = Main.rand.NextFloat(0.6f, 0.8f);
            }
        }
    }
}