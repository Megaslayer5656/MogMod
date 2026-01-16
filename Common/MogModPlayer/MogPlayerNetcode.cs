using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MogMod.Common.Systems;

namespace MogMod.Common.MogModPlayer
{
    public partial class MogPlayer : ModPlayer
    {
        public void SyncEssenceShift(int toWho, int fromWho)
        {
            MogPlayer mogPlayer = Main.player[fromWho].GetModPlayer<MogPlayer>();
            ModPacket packet = Mod.GetPacket(256);
            packet.Write((byte)MogModNetcode.MogModMessageType.EssenceShiftStackSync);
            if (Main.netMode == NetmodeID.Server)
            {
                packet.Write(fromWho);
            }
            packet.Write(mogPlayer.essenceShiftLevel);
            //Add stuff here about the player's stats changing due to essence shift (refer to intermediate netcode documentation on the tmodloader github repo wiki)
            packet.Send(toWho, fromWho);
        }

        internal void HandleEssenceShiftStack(BinaryReader reader, int fromWho)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                fromWho = reader.ReadInt32();
            }
            int essenceShiftLevelNetcode = reader.ReadInt32();
            //Add stuff here about the player's stats changing due to essence shift (refer to intermediate netcode documentation on the tmodloader github repo wiki)
            if (Main.netMode == NetmodeID.Server)
            {
                SyncEssenceShift(-1, fromWho);
            } else
            {
                MogPlayer mogPlayer = Main.player[fromWho].GetModPlayer<MogPlayer>();
                mogPlayer.essenceShiftLevel = essenceShiftLevelNetcode;
            }
        }
    }
}
