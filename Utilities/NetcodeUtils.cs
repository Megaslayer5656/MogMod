using MogMod.Common.MogModPlayer;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Utilities
{
    public static partial class MogModUtils
    {
        public static void WriteWhoAmI(this BinaryWriter writer, ModPlayer player) => WriteWhoAmI(writer, player?.Player);
        public static void WriteWhoAmI(this BinaryWriter writer, Player player)
        {
            byte whoAmI = (byte)(player?.whoAmI ?? Main.maxPlayers);
            writer.Write(whoAmI);
        }

        public static void WriteWhoAmI(this BinaryWriter writer, ModNPC npc) => WriteWhoAmI(writer, npc?.NPC);
        public static void WriteWhoAmI(this BinaryWriter writer, NPC npc)
        {
            byte whoAmI = (byte)(npc?.whoAmI ?? Main.maxNPCs);
            writer.Write(whoAmI);
        }

        public static MogPlayer ReadMogPlayer(this BinaryReader reader, bool nullOnInactive = true) => ReadPlayer(reader, nullOnInactive)?.MogMod() ?? null;
        public static Player ReadPlayer(this BinaryReader reader, bool nullOnInactive = true)
        {
            int index = reader.ReadByte();

            if (index >= Main.maxPlayers)
                return null;

            var player = Main.player[index];

            if (nullOnInactive && player.IsNullOrInactive())
                return null;

            return player;
        }

        public static NPCType ReadModNPC<NPCType>(this BinaryReader reader, bool nullOnInactive = true) where NPCType : ModNPC => ReadNPC(reader, nullOnInactive)?.ModNPC as NPCType;
        public static ModNPC ReadModNPC(this BinaryReader reader, bool nullOnInactive = true) => ReadNPC(reader, nullOnInactive)?.ModNPC ?? null;
        public static NPC ReadNPC(this BinaryReader reader, bool nullOnInactive = true)
        {
            int index = reader.ReadByte();

            if (index >= Main.maxNPCs)
                return null;

            var npc = Main.npc[index];

            if (nullOnInactive && npc.IsNullOrInactive())
                return null;

            return npc;
        }
    }
}
