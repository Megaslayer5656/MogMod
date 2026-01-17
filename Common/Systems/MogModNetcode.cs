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
    public class MogModNetcode
    {
        public static void HandlePacket(Mod mod, BinaryReader reader, int whoAmI)
        {
            try
            {
                MogModMessageType msgType = (MogModMessageType)reader.ReadByte();

                switch (msgType)
                {
                    case MogModMessageType.EssenceShiftStackSync:
                        Main.player[reader.ReadInt32()].GetModPlayer<MogPlayer>().HandleEssenceShiftStack(reader);
                        break;

                    case MogModMessageType.ShivasSync:
                        Main.player[reader.ReadInt32()].GetModPlayer<MogPlayer>().HandleShivas(reader);
                        break;
                }
            }
            catch (Exception e)
            {
                mod.Logger.Error("MogMod packet error: " + e);
            }
        }

        public enum MogModMessageType : byte
        {
            EssenceShiftStackSync,
            ShivasSync,
            Test2
        }
    }
}