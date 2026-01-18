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
    public partial class MogPlayer : ModPlayer //Hey Will, if you're looking at this file and are confused, go to the SyncShivas method and follow the path of comments I've set out for you to learn how this netcode stuff works.
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
            ModPacket packet = Mod.GetPacket(256); //Creates the packet. IMPORTANT: Info from packets needs to be read in the same order it is sent.
            MogPlayer mogPlayer = Player.GetModPlayer<MogPlayer>();

            packet.Write((byte)MogModMessageType.ShivasSync); //Needed for MogModNetcode.cs, lets the packet handler know what handle method to use.
            packet.Write(Player.whoAmI); //Also needed for MogModNetcode.cs, lets the packet handler know who sent the packet.
            packet.WriteVector2(position); //This is read in the method HandleShivas(), used for the doShivas() method.

            Player.SendPacket(packet, server); //Sends the packet, the packet is initially handled in MogMogNetcode.cs, then sent back to one of the handling methods in this file.
                                               //P.S. this method is a custom method in PlayerUtils.cs, see how it works there.
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
            Vector2 pos = reader.ReadVector2(); //Reads in the pos value
            if (Main.netMode == NetmodeID.Server) //If the server recieves the file, sync shivas again but through the server so it sends to all clients.
            {
                SyncShivas(true, pos);
            }
            doShivas(Player, pos); //This is how it actually syncs, using the position read in above, it creates the shivas effect on that player.
        }
    }
}
