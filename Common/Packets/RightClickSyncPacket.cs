using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using System.IO;
using Terraria;

namespace MogMod.Common.Packets
{
    // cfcm (copied from calamity mod)
    internal sealed class RightClickSyncPacket : MogPacket
    {
        public static RightClickSyncPacket Instance { get; private set; }
        public static void Send(MogPlayer player, int toClient = -1, int ignoreClient = -1)
        {
            if (player is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(player);
            packet.Write(player.mouseRight);
            packet.Send(toClient, ignoreClient);
        }
        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var player = packet.ReadMogPlayer();
            var rightClick = packet.ReadBoolean();

            if (player is null)
                return;

            player.mouseRight = rightClick;

            if (Main.dedServ)
                Send(player, ignoreClient: sender);
        }
    }
}