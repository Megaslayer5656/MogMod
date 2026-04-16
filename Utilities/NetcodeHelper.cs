using Terraria;
using Terraria.ModLoader;
using System.IO;
using Terraria.ID;
using MogMod.Common.MogModPlayer;
using System;
using MogMod.NPCs.Global;
using Microsoft.Xna.Framework;
using static MogMod.Common.Systems.MogModNetcode;
using Terraria.Audio;

namespace MogMod.Utilities
{
    public static partial class MogModUtils
    {
      public void NPCVelocitySync(NPC npc, Vector2 velocity, Vector2 position) //I'm making this as a simple npc velocity netcode helper method to use for npc ai. Not sure if this works yet as I'm at school and unable to test. Ensure this isn't ran by multiplayer clients when you call this method.
      {
        ModPacket packet = Mod.GetPacket();

        packet.Write((byte)MogModMessageType.NPCVelocitySync);
        packet.Write(npc.whoAmI);
        packet.WriteVector2(velocity);
        packet.WriteVector2(position);

        packet.Send();
      }

      public void SoundSync(SoundStyle sound) //I'll figure out how to do this one when I can actually test. This will be a easy netcode helper for syncing any sound, especially sounds triggered from npc ai. I'll likely have to send the properties of the sound and reconstruct the SoundStyle in the netcode file. Wonderful.
      {
        ModPacket packet = Mod.GetPacket();

        packet.Write((byte)MogModMessageType.SoundSync);
        //packet.Write(sound.SoundPath);
        //packet.Write(sound.Pitch);
        //packet.Write(sound.Volume);
        //packet.Write(sound.MaxInstances);

        packet.Send();
      }
    }
}
