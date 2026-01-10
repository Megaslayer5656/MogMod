using System;
using Terraria;
using Terraria.ModLoader;
namespace MogMod.Buffs.AccessoryAuras
{
    public class VladmirsBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 3;
            player.GetDamage(DamageClass.Generic) += .07f;
            player.lifeSteal += 35;
            player.manaRegen += (int)Math.Round(player.manaRegen * .05f);
        }
    }
}