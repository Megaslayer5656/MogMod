using System.Drawing.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Terraria.ModLoader;
using MogMod.Common.Systems;

namespace MogMod
{
    public class MogMod : Mod
    {
        public override void Load()
        {
            
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI) => MogModNetcode.HandlePacket(this, reader, whoAmI);
    }
}