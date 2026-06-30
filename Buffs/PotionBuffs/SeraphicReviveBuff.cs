using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Buffs.PotionBuffs
{
    public class SeraphicReviveBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}