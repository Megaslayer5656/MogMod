using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
namespace MogMod.Buffs.PotionBuffs
{
    public class BladeMailBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.thorns += 10f;

            // TODO: make a projectile that sticks out of the playerr and spawn dusts around that projectile
            if (Main.rand.NextBool(5))
            {
                int d = Dust.NewDust(player.position, player.width, player.height, DustID.Lead, player.velocity.X, player.velocity.Y);
                Main.dust[d].noGravity = true;
                Main.dust[d].scale -= 0.02f;
            }
        }
    }
}