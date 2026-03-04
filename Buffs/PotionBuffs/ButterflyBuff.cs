using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Buffs.PotionBuffs
{
    public class ButterflyBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.shadowDodge = true;
        }
    }
}
