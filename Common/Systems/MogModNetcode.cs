using Terraria;
using Terraria.ModLoader;
﻿using System.Collections.Generic;
using System.IO;
using Terraria.ID;
using MogMod.Utilities;
using MogMod.Common.MogModPlayer;
using System;
using MogMod.NPCs.Global;
using Microsoft.Xna.Framework;

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

                    case MogModMessageType.WingsOfLightSync:
                        Main.player[reader.ReadInt32()].GetModPlayer<MogPlayer>().HandleWingsOfLight(reader);
                        break;

                    case MogModMessageType.DuelistSync:
                        Main.player[reader.ReadInt32()].GetModPlayer<MogPlayer>().HandleDuelistGloves(reader);
                        break;

                    case MogModMessageType.ButterflySync:
                        Main.player[reader.ReadInt32()].GetModPlayer<MogPlayer>().HandleButterfly(reader);
                        break;

                    case MogModMessageType.ParrySync:
                        Main.player[reader.ReadInt32()].GetModPlayer<MogPlayer>().HandleParry(reader);
                        break;

                    case MogModMessageType.DragonInstallSync:
                        Main.player[reader.ReadInt32()].GetModPlayer<MogPlayer>().HandleDragonInstall(reader);
                        break;

                    case MogModMessageType.BleedProcTextSync:
                        {
                            Vector2 pos = reader.ReadVector2();
                            MogModGlobalNPC.doBloodFX(pos);
                            break;
                        }

                    case MogModMessageType.UltraCritTextSync:
                        Main.player[reader.ReadInt32()].GetModPlayer<MogPlayer>().HandleUltraCritText(reader); //This packet is sent directly from the ChaosBlade file.
                        break;

                    case MogModMessageType.AddBloodFromItem:
                        {
                            int npcID = reader.ReadInt32();
                            int playerID = reader.ReadInt32();
                            int itemType = reader.ReadInt32();

                            NPC npc = Main.npc[npcID];
                            Terraria.Player player = Main.player[playerID];
                            Item item = player.HeldItem;

                            if (npc.TryGetGlobalNPC<MogModGlobalNPC>(out var g))
                            {
                                g.AddItemBlood(npc, player, item);
                            }

                            break;
                        }

                    case MogModMessageType.AddBloodFromProjectile:
                        {
                            int npcID = reader.ReadInt32();
                            int bloodToAdd = reader.ReadInt32();

                            NPC npc = Main.npc[npcID];

                            MogModGlobalNPC globalNPC = npc.GetGlobalNPC<MogModGlobalNPC>();
                            if (Main.netMode == NetmodeID.Server)
                            {
                                globalNPC.AddProjectileBlood(npc, bloodToAdd);
                            }

                            break;
                        }

                    case MogModMessageType.ProjParrySync:
                        {
                            int ownerID = reader.ReadInt32();
                            int projID = reader.ReadInt32();
                            int targetID = reader.ReadInt32();

                            Projectile projectile = MogModUtils.FindProjectileByIdentity(projID, Main.player[ownerID].whoAmI);

                            if (projectile != null)
                            {
                                projectile.velocity.X = projectile.velocity.X * -1;
                                projectile.velocity.Y = projectile.velocity.Y * -1;
                                projectile.friendly = true;
                                projectile.hostile = false;
                                projectile.damage *= 5;
                            }

                            MogPlayer mogPlayer = Main.player[targetID].GetModPlayer<MogPlayer>();

                            if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                mogPlayer.doParryFX(Main.player[targetID].Center);
                            }

                            
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                mod.Logger.Error("MogMod packet error: " + e);
            }
        }
        // w speed
        public enum MogModMessageType : byte //This is where you create the message types
        {
            EssenceShiftStackSync,
            ShivasSync,
            WingsOfLightSync,
            DuelistSync,
            ButterflySync,
            ParrySync,
            DragonInstallSync,
            BleedProcTextSync,
            TrueStrikeProcTextSync,
            BashProcTextSync,
            UltraCritTextSync,
            AddBloodSync,
            AddBloodFromItem,
            AddBloodFromProjectile,
            ProjParrySync
        }
    }
}