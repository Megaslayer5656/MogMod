using System;
using Terraria;
using Terraria.ModLoader;
namespace MogMod.Buffs.PotionBuffs
{
    public class ClarityBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.manaRegen += (int)Math.Round(player.manaRegen * .3f);
            player.manaRegenDelay -= 5f;
        }
    }
}