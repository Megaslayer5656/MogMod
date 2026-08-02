using MogMod.Utilities;
using System.IO;
using Terraria;
using Terraria.ID;

namespace MogMod.Common.Packets
{
    // copied from calamity mod
    internal sealed class SyncNPCMotionDataToServerPacket : MogPacket
    {
        public static SyncNPCMotionDataToServerPacket Instance { get; private set; }

        public static void Send(NPC npc, int toClient = -1, int ignoreClient = -1)
        {
            if (npc is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(npc);
            packet.WriteVector2(npc.Center);
            packet.WriteVector2(npc.velocity);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var npc = packet.ReadNPC();
            var center = packet.ReadVector2();
            var velocity = packet.ReadVector2();
            if (Main.dedServ && npc is not null)
            {
                npc.Center = center;
                npc.velocity = velocity;
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
            }
        }
    }
}
