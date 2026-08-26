using Terraria;
using Terraria.ModLoader;
using System.IO;
using Terraria.ID;
using MogMod.Common.MogModPlayer;
using System;
using MogMod.NPCs.Global;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using MogMod.Utilities;

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
                        Main.player[reader.ReadInt32()].MogMod().HandleEssenceShiftStack(reader);
                        break;

                    case MogModMessageType.ShivasSync: //If the message type is ShivasSync:
                        Main.player[reader.ReadInt32()].MogMod().HandleShivas(reader); //Sends the packet to the ShivasHandler in MogPlayerNetcode.cs
                        break;

                    case MogModMessageType.WingsOfLightSync:
                        Main.player[reader.ReadInt32()].MogMod().HandleWingsOfLight(reader);
                        break;

                    case MogModMessageType.DuelistSync:
                        Main.player[reader.ReadInt32()].MogMod().HandleDuelistGloves(reader);
                        break;

                    case MogModMessageType.ButterflySync:
                        Main.player[reader.ReadInt32()].MogMod().HandleButterfly(reader);
                        break;

                    case MogModMessageType.ParrySync:
                        Main.player[reader.ReadInt32()].MogMod().HandleParry(reader);
                        break;

                    case MogModMessageType.DragonInstallSync:
                        Main.player[reader.ReadInt32()].MogMod().HandleDragonInstall(reader);
                        break;

                    case MogModMessageType.BleedProcTextSync:
                        int bloodID = reader.ReadInt32();
                        int bloodDMG = reader.ReadInt32();

                        NPC bloodNPC = Main.npc[bloodID];
                        bloodNPC.MogMod().BloodFX(bloodNPC, bloodDMG);
                        break;
                    case MogModMessageType.ToxicProcTextSync:
                        int toxicProcID = reader.ReadInt32();

                        NPC toxicProcNPC = Main.npc[toxicProcID];
                        toxicProcNPC.MogMod().ToxicFX(toxicProcNPC);
                        break;
                    case MogModMessageType.UltraCritTextSync:
                        int ultraCritID = reader.ReadInt32();

                        NPC ultraCritNPC = Main.npc[ultraCritID];
                        ultraCritNPC.MogMod().UltraCritFX(ultraCritNPC);
                        break;
                    case MogModMessageType.BashProcTextSync:
                        int bashID = reader.ReadInt32();

                        NPC bashNPC = Main.npc[bashID];
                        bashNPC.MogMod().BashFX(bashNPC);
                        break;
                    case MogModMessageType.TrueStrikeProcTextSync:
                        Vector2 strikePos = reader.ReadVector2();
                        MogModGlobalNPC.TrueStrikeFX(strikePos);
                        break;

                    case MogModMessageType.DamageReducedTextSync:
                        int damageReduceProcID = reader.ReadInt32();

                        Player damageReduceProcPlayer = Main.player[damageReduceProcID];
                        damageReduceProcPlayer.MogMod().DamageReducedFX(damageReduceProcPlayer);
                        break;

                    case MogModMessageType.DamageBlockedTextSync:
                        int damageBlockProcID = reader.ReadInt32();

                        Player damageBlockProcPlayer = Main.player[damageBlockProcID];
                        damageBlockProcPlayer.MogMod().DamageBlockedFX(damageBlockProcPlayer);
                        break;

                    case MogModMessageType.AddBloodFromItem:
                        {
                            int npcID = reader.ReadInt32();
                            int playerID = reader.ReadInt32();
                            int itemType = reader.ReadInt32();

                            NPC npc = Main.npc[npcID];
                            Player player = Main.player[playerID];
                            Item item = player.HeldItem;

                            if (Main.netMode == NetmodeID.Server)
                            {
                                if (npc.TryGetGlobalNPC<MogModGlobalNPC>(out var g))
                                {
                                    g.AddItemBlood(npc, player, item);
                                }
                            }

                            break;
                        }

                    case MogModMessageType.AddBloodFromProjectile:
                        {
                            int npcID = reader.ReadInt32();
                            int playerID = reader.ReadInt32();
                            int blood = reader.ReadInt32();

                            NPC npc = Main.npc[npcID];
                            Player player = Main.player[playerID];

                            MogModGlobalNPC globalNPC = npc.MogMod();
                            if (Main.netMode == NetmodeID.Server)
                            {
                                globalNPC.AddProjectileBlood(npc, player, blood);
                            }

                            break;
                        }

                    case MogModMessageType.AddToxicFromItem:
                        {
                            int npcID = reader.ReadInt32();
                            int playerID = reader.ReadInt32();

                            NPC npc = Main.npc[npcID];
                            Player player = Main.player[playerID];

                            if (npc.TryGetGlobalNPC<MogModGlobalNPC>(out var g))
                            {
                                g.AddItemToxic(player);
                            }

                            break;
                        }

                    case MogModMessageType.AddToxicFromProjectile:
                        {
                            int npcID = reader.ReadInt32();
                            int playerID = reader.ReadInt32();

                            Player player = Main.player[playerID];
                            NPC npc = Main.npc[npcID];

                            MogModGlobalNPC globalNPC = npc.MogMod();
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                globalNPC.AddProjectileToxic(player);
                            }

                            break;
                        }

                    case MogModMessageType.MarkerProjSync:
                        {
                            Main.player[reader.ReadInt32()].MogMod().HandleMarkerProj(reader);
                            break;
                        }

                    case MogModMessageType.MarkerProjOutSync:
                        {
                            Main.player[reader.ReadInt32()].MogMod().HandleMarkerProjOut(reader);
                            break;
                        }

                    case MogModMessageType.ProjParrySync:
                        {
                            Main.player[reader.ReadInt32()].MogMod().HandleProjParry(reader);
                            break;
                        }

                    case MogModMessageType.NPCVelocitySync:
                        {
                            int npcID = reader.ReadInt32();
                            Vector2 velocity = reader.ReadVector2();
                            Vector2 center = reader.ReadVector2();

                            NPC npc = Main.npc[npcID];

                            if (Main.dedServ && npc is not null)
                            {
                                npc.Center = center;
                                npc.velocity = velocity;
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            }
                            break;
                        }

                    case MogModMessageType.SoundSync:
                        {
                            string path = reader.ReadString();
                            float pitch = reader.ReadSingle();
                            float volume = reader.ReadSingle();
                            int maxInstances = reader.ReadInt32();
                            Vector2 position = reader.ReadVector2();

                            SoundStyle sound = new SoundStyle(path)
                            {
                                Volume = volume,
                                PitchVariance = pitch,
                                MaxInstances = maxInstances,
                            };

                            if (Main.netMode != NetmodeID.Server)
                            {
                                SoundEngine.PlaySound(sound, position);
                            }
                            
                            break;
                        }

                    case MogModMessageType.HellEpstein:
                        int hellEpsteinID = reader.ReadInt32();
                        NPC hellEpsteinNPC = Main.npc[hellEpsteinID];
                        hellEpsteinNPC.MogMod().MakeHellEpstein(hellEpsteinNPC);
                        break;

                    case MogModMessageType.OverloadingElite:
                        int overloadingID = reader.ReadInt32();
                        NPC overloadingNPC = Main.npc[overloadingID];
                        overloadingNPC.MogMod().MakeOverloading(overloadingNPC);
                        break;
                    case MogModMessageType.BlazingElite:
                        int blazingID = reader.ReadInt32();
                        NPC blazingNPC = Main.npc[blazingID];
                        blazingNPC.MogMod().MakeBlazing(blazingNPC);
                        break;
                    case MogModMessageType.GildedElite:
                        int gildedID = reader.ReadInt32();
                        NPC gildedNPC = Main.npc[gildedID];
                        gildedNPC.MogMod().MakeGilded(gildedNPC);
                        break;
                    case MogModMessageType.MendingElite:
                        int mendingID = reader.ReadInt32();
                        NPC mendingNPC = Main.npc[mendingID];
                        mendingNPC.MogMod().MakeMending(mendingNPC);
                        break;
                    case MogModMessageType.ToxicElite:
                        int toxicID = reader.ReadInt32();
                        NPC toxicNPC = Main.npc[toxicID];
                        toxicNPC.MogMod().MakeToxic(toxicNPC);
                        break;
                }
            }
            catch (Exception e)
            {
                mod.Logger.Error("MogMod packet error: " + e);
            }
        }
        // w speed
        public static void SyncWorld()
        {
            if (Main.dedServ)
                NetMessage.SendData(MessageID.WorldData);
        }
        public enum MogModMessageType : byte //This is where you create the message types
        {
            EssenceShiftStackSync,
            ShivasSync,
            WingsOfLightSync,
            DuelistSync,
            ButterflySync,
            ParrySync,
            DragonInstallSync,
            TrueStrikeProcTextSync,
            BashProcTextSync,
            UltraCritTextSync,
            BleedProcTextSync,
            AddBloodFromItem,
            AddBloodFromProjectile,
            ToxicProcTextSync,
            AddToxicFromItem,
            AddToxicFromProjectile,
            DamageReducedTextSync,
            DamageBlockedTextSync,
            ProjParrySync,
            MarkerProjSync,
            MarkerProjOutSync,
            NPCVelocitySync,
            SoundSync,
            HellEpstein,
            OverloadingElite,
            BlazingElite,
            GildedElite,
            MendingElite,
            ToxicElite
        }
    }
}
