using Terraria;
using Terraria.ModLoader;
﻿using System.Collections.Generic;
using System.IO;
using Terraria.ID;
using MogMod.Utilities;

namespace MogMod.Common.Systems
{
    public class MogModNetcode
    {
        public static void HandlePacket(Mod mod, BinaryReader reader, int whoAmI)
        {
            //TODO: Add try-catch for debugging
            MogModMessageType msgType = (MogModMessageType)reader.ReadByte();
            switch (msgType)
            {
                case MogModMessageType.EssenceShiftStackSync:
                    Main.player[reader.ReadInt32()].MogMod().HandleEssenceShiftStack(reader, whoAmI);
                    break;
            }
        }

        public enum MogModMessageType : byte
        {
            EssenceShiftStackSync,
            Test,
            Test2
        }
    }
}