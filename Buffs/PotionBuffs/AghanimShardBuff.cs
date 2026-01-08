using System;
using Terraria;
using Terraria.ModLoader;
namespace MogMod.Buffs.PotionBuffs
{
    public class AghanimShardBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statManaMax2 += 100;
            player.manaRegen += (int)Math.Round(player.manaRegen * .8f);
            player.manaRegenDelay = 0f;
            player.GetDamage(DamageClass.Magic) += .20f;
        }
    }
}