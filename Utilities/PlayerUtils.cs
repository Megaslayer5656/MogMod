using Terraria.ModLoader;
using Terraria;

namespace MogMod.Utilities
{
    public static class PlayerUtils
    {
        public static void SendPacket(this Player player, ModPacket packet, bool server) //Thank you for this idea Calamity Mod, we love you!
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
