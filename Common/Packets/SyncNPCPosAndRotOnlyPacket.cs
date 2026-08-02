using MogMod.Utilities;
using System;
using System.IO;
using Terraria;

namespace MogMod.Common.Packets
{
    // copied from calamity mod
    internal sealed class SyncNPCPosAndRotOnlyPacket : MogPacket
    {
        public static SyncNPCPosAndRotOnlyPacket Instance { get; private set; }

        public static void Send(NPC npc, int toClient = -1, int ignoreClient = -1)
        {
            if (npc is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(npc);
            packet.WriteVector2(npc.position);
            packet.Write((Half)npc.rotation); //rotation unit is radian (-π/2 ≤ rotation ≤ π/2) so Half precision should works
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var npc = packet.ReadNPC();
            var position = packet.ReadVector2();
            var rotation = (float)packet.ReadHalf();

            if (npc is null)
                return;

            npc.position = position;
            npc.rotation = rotation;

            if (Main.dedServ)
            {
                Send(npc, ignoreClient: sender);
            }
        }
    }
}
