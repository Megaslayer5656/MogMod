using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Buffs.AccessoryAuras
{
    public class GuardianGreavesAura : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 6;
            player.statDefense += 4;
            player.statLifeMax2 += 20;
            player.statManaMax2 += 50;
            player.GetDamage(DamageClass.Generic) += .10f;
        }
    }
}