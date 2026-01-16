using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using MogMod.Common.MogModPlayer;

namespace MogMod.Utilities
{
    public static class PlayerUtils
    {
        public static void SendPacket(this Player player, ModPacket packet, bool server)
        {
            // Client: Send the packet only to the host.
            if (!server)
                packet.Send();

            // Server: Send the packet to every OTHER client.
            else
                packet.Send(-1, player.whoAmI);
        }
    }
}
