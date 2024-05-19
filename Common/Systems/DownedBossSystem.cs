using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MogMod.Common.Systems
{
    public class DownedBossSystem : ModSystem
    {
        public static bool downedDabDad = false;
        public override void ClearWorld()
        {
            downedDabDad = false;
        }
        public override void SaveWorldData(TagCompound tag)
        {
            if (downedDabDad)
            {
                tag["downedDabDad"] = true;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedDabDad = tag.ContainsKey("downedDabDad");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = downedDabDad;
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedDabDad = flags[0];
        }
    }
}