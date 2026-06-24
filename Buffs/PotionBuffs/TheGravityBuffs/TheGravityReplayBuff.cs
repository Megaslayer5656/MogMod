using Terraria;
using Terraria.ModLoader;

namespace MogMod.Buffs.PotionBuffs.TheGravityBuffs
{
    public class TheGravityReplayBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
        }
    }
}