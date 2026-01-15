using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Common.MogModPlayer
{
    public partial class MogPlayer : ModPlayer
    { 
        //public void SyncEssenceShift(bool server)
        //{
        //    ModPacket packet = Mod.GetPacket(256);
        //    packet.Write((byte)MogModMessageType.EssenceShiftStackSync);
              //Add stuff here about the player's stats changing due to essence shift (refer to intermediate netcode documentation on the tmodloader github repo wiki)
        //    Player.SendPacket(packet, server);
        //}

        //internal void HandleEssenceShiftStack(BinaryReader reader)
        //{
            //Add stuff here about the player's stats changing due to essence shift (refer to intermediate netcode documentation on the tmodloader github repo wiki)
        //    if (Main.netMode == NetmodeID.Server)
        //        SyncEssenceShift(true);
        //}
    }
}
