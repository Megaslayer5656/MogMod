using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MogMod.Utilities;
using Microsoft.Xna.Framework;
using static MogMod.Common.Systems.MogModNetcode;
using MogMod.NPCs.Global;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Common.Packets;

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

        public void SyncWingsOfLight(bool server, Vector2 position)
        {
            ModPacket packet = Mod.GetPacket(256);
            MogPlayer mogPlayer = Player.GetModPlayer<MogPlayer>();

            packet.Write((byte)MogModMessageType.WingsOfLightSync);
            packet.Write(Player.whoAmI);
            packet.WriteVector2(position);

            Player.SendPacket(packet, server);
        }

        public void SyncDuelistGloves(bool server, Vector2 position)
        {
            ModPacket packet = Mod.GetPacket(256);
            MogPlayer mogPlayer = Player.GetModPlayer<MogPlayer>();

            packet.Write((byte)MogModMessageType.DuelistSync);
            packet.Write(Player.whoAmI);
            packet.WriteVector2(position);

            Player.SendPacket(packet, server);
        }

        public void SyncButterfly(bool server)
        {
            ModPacket packet = Mod.GetPacket(256);

            packet.Write((byte)MogModMessageType.ButterflySync);
            packet.Write(Player.whoAmI);

            Player.SendPacket(packet, server);
        }

        public void SyncParry(bool server, Vector2 pos)
        {
            ModPacket packet = Mod.GetPacket(256);
            MogPlayer mogPlayer = Player.GetModPlayer<MogPlayer>();

            packet.Write((byte)MogModMessageType.ParrySync);
            packet.Write(Player.whoAmI);
            packet.WriteVector2(pos);

            Player.SendPacket(packet, server);
        }

        public void SyncDragonInstall(bool server)
        {
            ModPacket packet = Mod.GetPacket(256);
            MogPlayer mogPlayer = Player.GetModPlayer<MogPlayer>();

            packet.Write((byte)MogModMessageType.DragonInstallSync);
            packet.Write(Player.whoAmI);
            packet.Write(mogPlayer.dragonInstallActive);

            Player.SendPacket(packet, server);
        }

        public void SyncMarkerProj(bool server, NPC npc, Terraria.Player player, Item item, Vector2 velocity, float rotation)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MogModMessageType.MarkerProjSync);
            packet.Write(Player.whoAmI);
            packet.Write(npc.whoAmI);
            packet.Write(player.whoAmI);
            packet.Write(item.type);
            packet.WriteVector2(velocity);
            packet.Write(rotation);
            packet.Send();
        }

        public void SyncMarkerProjOut(bool server, Terraria.Player player)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MogModMessageType.MarkerProjSync);
            packet.Write(Player.whoAmI);
            packet.Write(player.whoAmI);
            packet.Send();
        }

        public void SyncProjParry(bool server, int ownerID, Terraria.Player target, Projectile projectile)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MogModMessageType.ProjParrySync);
            packet.Write(Player.whoAmI);
            packet.Write(ownerID);
            packet.Write(projectile.identity);
            packet.Write(target.whoAmI);
            packet.Send();
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

        internal void HandleWingsOfLight(BinaryReader reader)
        {
            Vector2 pos = reader.ReadVector2();
            if (Main.netMode == NetmodeID.Server)
            {
                SyncWingsOfLight(true, pos);
            }
            doWingsOfLight(Player, pos);
        }

        internal void HandleDuelistGloves(BinaryReader reader)
        {
            Vector2 pos = reader.ReadVector2();
            if (Main.netMode == NetmodeID.Server)
            {
                SyncDuelistGloves(true, pos);
            }
            doDuelistGloves(Player, pos);
        }

        internal void HandleButterfly(BinaryReader reader)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                SyncButterfly(true);
            }
            doButterfly(Player);
        }

        internal void HandleParry(BinaryReader reader)
        {
            Vector2 pos = reader.ReadVector2();
            if (Main.netMode == NetmodeID.Server)
            {
                SyncParry(true, pos);
            }
            doParryFX(pos);
        }

        internal void HandleDragonInstall(BinaryReader reader)
        {
            bool install = reader.ReadBoolean();
            if (Main.netMode == NetmodeID.Server)
            {
                SyncDragonInstall(true);
            }

            if (install)
            {
                enterDragonInstall(Player);
            }
            else
            {
                exitDragonInstall(Player);
            }
        }

        internal void HandleMarkerProj(BinaryReader reader)
        {
            int npcID = reader.ReadInt32();
            int playerID = reader.ReadInt32();
            int itemType = reader.ReadInt32();
            Vector2 velocity = reader.ReadVector2();
            float rotation = reader.ReadSingle();

            NPC npc = Main.npc[npcID];
            Terraria.Player player = Main.player[playerID];
            Item item = new Item();
            item.SetDefaults(itemType);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                MogModGlobalNPC.SpawnMarkerProjectile(npc, player, item, velocity, rotation);
            } 
            else if (Main.netMode == NetmodeID.Server)
            {
                SyncMarkerProj(true, npc, player, item, velocity, rotation);
            }
        }

        internal void HandleMarkerProjOut(BinaryReader reader)
        {
            int playerID = reader.ReadInt32();

            Terraria.Player player = Main.player[playerID];
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.markerProjOut = false;
            if (Main.netMode == NetmodeID.Server)
            {
                SyncMarkerProjOut(true, player);
            }
        }

        internal void HandleProjParry(BinaryReader reader)
        {
            int ownerID = reader.ReadInt32();
            int projID = reader.ReadInt32();
            int targetID = reader.ReadInt32();

            Projectile projectile = MogModUtils.FindProjectileByIdentity(projID, ownerID);

            MogPlayer mogPlayer = Main.player[targetID].GetModPlayer<MogPlayer>();

            if (Main.netMode == NetmodeID.Server)
            {
                SyncProjParry(true, ownerID, Main.player[targetID], projectile);
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                mogPlayer.doParryFX(Main.player[targetID].Center);
                MogModGlobalProjectile.ParryProjectile(projectile, targetID);
            }
        }

        internal void MousePositionSync()
        {
            MousePositionSyncPacket.Send(this);
        }

        internal void MouseRotationSync()
        {
            MouseRotationSyncPacket.Send(this);
        }

        internal void MouseRightClickSync()
        {
            RightClickSyncPacket.Send(this);
        }
    }
}