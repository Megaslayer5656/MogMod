using Terraria.ModLoader;
using Terraria;

namespace MogMod.Buffs.Debuffs
{
    public class VonDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen -= 50;
            player.moveSpeed -= .15f;
            player.GetDamage(DamageClass.Generic) -= .15f;
            //TODO: Add more effects
        }
    }
}
