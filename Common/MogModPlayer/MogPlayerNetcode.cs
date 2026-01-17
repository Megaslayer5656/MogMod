using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MogMod.Common.Systems;
using static MogMod.Common.Systems.MogModNetcode;
using MogMod.Utilities;
using Terraria.GameContent.UI;
using Terraria.Audio;
using Microsoft.Xna.Framework;

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

        public void SyncShivas(bool server, Vector2 position)
        {
            ModPacket packet = Mod.GetPacket(256);
            MogPlayer mogPlayer = Player.GetModPlayer<MogPlayer>();

            packet.Write((byte)MogModMessageType.ShivasSync);
            packet.Write(Player.whoAmI);
            packet.WriteVector2(position);

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

        internal void HandleShivas(BinaryReader reader)
        {
            Vector2 pos = reader.ReadVector2();
            if (Main.netMode == NetmodeID.Server) 
            {
                SyncShivas(true, pos);
            }
            doShivas(Player, pos);

        }
    }
}
