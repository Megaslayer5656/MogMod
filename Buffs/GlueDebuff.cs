using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Buffs
{
    public class GlueDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statLifeMax2 -= player.statLifeMax - 20;
            player.GetAttackSpeed(DamageClass.Generic) -= 95 / 100f;
            player.moveSpeed -= 95 / 100f;
            player.GetDamage(DamageClass.Generic) -= 98 / 100f;
            player.lifeRegen -= 20;
        }
    }
}