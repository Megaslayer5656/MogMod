using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using System;
using System.IO;
using Terraria;

namespace MogMod.Common.Packets
{
    // calamity mod
    internal sealed class MouseRotationSyncPacket : MogPacket
    {
        public static MouseRotationSyncPacket Instance { get; private set; }
        public static void Send(MogPlayer player, int toClient = -1, int ignoreClient = -1)
        {
            if (player is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(player);
            packet.Write((Half)player.mouseRotationFromPlayer);
            packet.Send(toClient, ignoreClient);
        }
        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var player = packet.ReadMogPlayer();
            var rotation = (float)packet.ReadHalf();

            if (player is null)
                return;

            player.mouseRotationFromPlayer = rotation;
            player.mouseWorldDeltaFromPlayer = rotation.ToRotationVector2();

            if (Main.dedServ)
                Send(player, ignoreClient: sender);
        }
    }
}