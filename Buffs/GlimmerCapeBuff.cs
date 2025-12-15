using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Buffs
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
        }
    }
}