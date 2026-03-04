using System;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Buffs.AccessoryAuras
{
    public class WraithPactAura : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 7;
            player.GetDamage(DamageClass.Generic) += .13f;
            player.lifeSteal *= 1.8f;
            player.manaRegen += (int)Math.Round(player.manaRegen * .1f);
        }
    }
}