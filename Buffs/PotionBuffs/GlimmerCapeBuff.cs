using System;
using Terraria;
using Terraria.ModLoader;
namespace MogMod.Buffs.PotionBuffs
{
    public class GlimmerCapeBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.invis = true;
            player.moveSpeed += 0.25f;
            player.manaRegen += (int)Math.Round(player.manaRegen * .3f);
            player.manaRegenDelay -= 4f;
        }
    }
}