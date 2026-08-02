using MogMod.Utilities;
using System.IO;
using Terraria;
using Terraria.DataStructures;

namespace MogMod.Common.Packets
{
    // copied from calamity mod
    internal sealed class SpawnNPCOnPlayerPacket : MogPacket
    {
        public static SpawnNPCOnPlayerPacket Instance { get; private set; }

        public static void Send(Player player, int x, int y, int npcType, int toClient = -1, int ignoreClient = -1)
        {
            if (player is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(player);
            packet.Write(x);
            packet.Write(y);
            packet.Write(npcType);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var player = packet.ReadPlayer();
            var x = packet.ReadInt32();
            var y = packet.ReadInt32();
            var npcType = packet.ReadInt32();

            if (player is null)
                return;

            if (Main.dedServ)
            {
                int spawnedNPC = NPC.NewNPC(new EntitySource_WorldEvent(), x, y, npcType, Target: player.whoAmI);
                if (spawnedNPC >= Main.maxNPCs)
                    return;

                MoreMogNetcode.SyncNPC(spawnedNPC);
            }
        }
    }
}