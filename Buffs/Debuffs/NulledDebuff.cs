using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Buffs.Debuffs
{
    public class NulledDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.nulledDebuff = true;
            if (Main.rand.NextBool(3))
            {
                Vector2 dustCorner = player.position - 2f * Vector2.One;
                Vector2 dustVel = player.velocity + new Vector2(0f, Main.rand.NextFloat(-5f, -1f));
                int dust = Dust.NewDust(dustCorner, player.width + 4, player.height + 4, 192, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 1.4f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.75f;
                Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.85f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 0.2f;
                }
            }
        }
    }
}