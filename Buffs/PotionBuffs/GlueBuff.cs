using Terraria;
using Terraria.ModLoader;
namespace MogMod.Buffs.PotionBuffs
{
    public class GlueBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statLifeMax2 += player.statLifeMax + 180;
            player.GetAttackSpeed(DamageClass.Generic) += 175 / 100f;
            player.moveSpeed += 175 / 100f;
            player.GetDamage(DamageClass.Generic) += 199 / 100f;
            player.lifeRegen += 25;
        }
    }
}