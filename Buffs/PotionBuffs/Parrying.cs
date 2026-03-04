using Terraria;
using Terraria.ModLoader;

namespace MogMod.Buffs.PotionBuffs
{
    public class Parrying : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }
    }
}