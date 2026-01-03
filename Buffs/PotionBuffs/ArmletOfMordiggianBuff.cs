using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Buffs.PotionBuffs
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
            float dim = .02f;
            Lighting.AddLight(player.Center, 136f * dim, 8f * dim, 8f * dim);
        }
    }
}
