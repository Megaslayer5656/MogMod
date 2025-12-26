using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Buffs
{
    public class CheeseBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 10;
            player.statDefense += 20;
            player.statLifeMax2 += 50;
            player.statManaMax2 += 100;
        }
    }
}