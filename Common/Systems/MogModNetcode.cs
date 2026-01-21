using Terraria;
using Terraria.ModLoader;
﻿using System.Collections.Generic;
using System.IO;
using Terraria.ID;
using MogMod.Utilities;
using MogMod.Common.MogModPlayer;
using System;

namespace MogMod.Common.Systems
{
    public class MogModNetcode //Huge thanks to the Calamity Mod public mirror on github, it really helped me get an idea of how all this stuff works. (P.S. My gf loves the Sylvestaff from Calamity, whoever made that is cool)
    {
        public static void HandlePacket(Mod mod, BinaryReader reader, int whoAmI)
        {
            try
            {
                MogModMessageType msgType = (MogModMessageType)reader.ReadByte(); //Reads in the message type (you can create message types in the enum MogModMessageType below in this file

                switch (msgType) //Depending on the message type used in MogPlayerNetcode.cs, this will send the packet to the corresponding handler in MogPlayerNetcode.cs
                {
                    case MogModMessageType.EssenceShiftStackSync:
                        Main.player[reader.ReadInt32()].GetModPlayer<MogPlayer>().HandleEssenceShiftStack(reader);
                        break;

                    case MogModMessageType.ShivasSync: //If the message type is ShivasSync:
                        Main.player[reader.ReadInt32()].GetModPlayer<MogPlayer>().HandleShivas(reader); //Sends the packet to the ShivasHandler in MogPlayerNetcode.cs
                        break;

                    case MogModMessageType.ButterflySync:
                        Main.player[reader.ReadInt32()].GetModPlayer<MogPlayer>().HandleButterfly(reader);
                        break;

                    case MogModMessageType.ParrySync:
                        Main.player[reader.ReadInt32()].GetModPlayer<MogPlayer>().HandleParry(reader);
                        break;
                }
            }
            catch (Exception e)
            {
                mod.Logger.Error("MogMod packet error: " + e);
            }
        }

        public enum MogModMessageType : byte //This is where you create the message types
        {
            EssenceShiftStackSync,
            ShivasSync,
            ButterflySync,
            ParrySync
        }
    }
}