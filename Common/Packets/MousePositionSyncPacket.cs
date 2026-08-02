using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using System.IO;
using Terraria;

namespace MogMod.Common.Packets
{
    internal sealed class MousePositionSyncPacket : MogPacket
    {
        public static MousePositionSyncPacket Instance { get; private set; }
        public static void Send(MogPlayer player, int toClient = -1, int ignoreClient = -1)
        {
            if (player is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(player);
            packet.Write((short)player.mouseWorldDeltaFromPlayer.X);
            packet.Write((short)player.mouseWorldDeltaFromPlayer.Y);
            packet.Send(toClient, ignoreClient);
        }
        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var player = packet.ReadMogPlayer();
            var deltaX = packet.ReadInt16();
            var deltaY = packet.ReadInt16();

            if (player is null)
                return;

            var delta = new Vector2(deltaX, deltaY);
            player.mouseWorldDeltaFromPlayer = delta;
            player.mouseRotationFromPlayer = delta.ToRotation();

            if (Main.dedServ)
                Send(player, ignoreClient: sender);
        }
    }
}