using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Buffs
{
    public class ArmletOfMordiggianBuff : ModBuff
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
            player.GetAttackSpeed(DamageClass.Generic) += 0.20f;
            player.GetDamage(DamageClass.Generic) += .20f;
        }
    }
}
