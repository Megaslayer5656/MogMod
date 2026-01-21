using MogMod.Common.MogModPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Buffs.Debuffs
{
    public class ParrySlow : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.controlUp = false;
            player.controlDown = false;
            player.controlLeft = false;
            player.controlRight = false;
            player.controlJump = false;
            player.controlHook = false;
            player.controlMount = false;
            if (player.velocity.Y > 15f)
                player.velocity.Y = 15f;
            player.shadowDodge = false;
        }
    }
}
