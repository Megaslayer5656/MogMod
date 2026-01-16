using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MogMod.Common.Systems;
using static MogMod.Common.Systems.MogModNetcode;
using MogMod.Utilities;
using Terraria.GameContent.UI;

namespace MogMod.Common.MogModPlayer
{
    public partial class MogPlayer : ModPlayer
    {
        public void SyncEssenceShift(bool server)
        {
            ModPacket packet = Mod.GetPacket(256);
            MogPlayer mogPlayer = Player.GetModPlayer<MogPlayer>();

            packet.Write((byte)MogModMessageType.EssenceShiftStackSync);
            packet.Write(Player.whoAmI);
            packet.Write(mogPlayer.essenceShiftLevel);
            Player.SendPacket(packet, server);
        }

        internal void HandleEssenceShiftStack(BinaryReader reader)
        {
            essenceShiftLevel = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
            {
                SyncEssenceShift(true);
            }
        }
    }
}
