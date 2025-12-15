using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Buffs
{
    public class GlimmerCapeDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
        }
    }
}