using MogMod.Items.Consumables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MogMod.Common.Player
{
    public class MogPlayerStatIncrease : ModPlayer
    {
        //public int aghanimShard;

        //// modifies player health and mana
        //public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        //{
        //    // incase theres gonna be health boost items
        //    health = StatModifier.Default;
        //    //health.Base = bOrange.ToInt() * BloodOrange.LifeBoost
        //    mana = StatModifier.Default;
        //    mana.Base = aghanimShard * AghanimShard.ManaBoost;
        //}
        //public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        //{
        //    ModPacket packet = Mod.GetPacket();
        //    // do something in networking
        //    //packet.Write((byte)MogMod.MessageType.MogPlayerStatIncreaseSync);
        //    packet.Write((byte)Player.whoAmI);
        //    // for health stat buffs
        //    //packet.Write((byte)exampleLifeFruits);
        //    packet.Write((byte)aghanimShard);
        //    packet.Send(toWho, fromWho);
        //}

        //// Called in MogMod.Networking.cs
        //public void ReceivePlayerSync(BinaryReader reader)
        //{
        //    //exampleLifeFruits = reader.ReadByte();
        //    aghanimShard = reader.ReadByte();
        //}

        //public override void CopyClientState(ModPlayer targetCopy)
        //{
        //    MogPlayerStatIncrease clone = (MogPlayerStatIncrease)targetCopy;
        //    //clone.exampleLifeFruits = exampleLifeFruits;
        //    clone.aghanimShard = aghanimShard;
        //}

        //public override void SendClientChanges(ModPlayer clientPlayer)
        //{
        //    MogPlayerStatIncrease clone = (MogPlayerStatIncrease)clientPlayer;

        //    if (aghanimShard != clone.aghanimShard) // exampleLifeFruits != clone.exampleLifeFruits || 
        //    {
        //        // This example calls SyncPlayer to send all the data for this ModPlayer when any change is detected, but if you are dealing with a large amount of data you should try to be more efficient and use custom packets to selectively send only specific data that has changed.
        //        SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        //    }
        //}

        //// NOTE: The tag instance provided here is always empty by default.
        //// Read https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound to better understand Saving and Loading data.
        //public override void SaveData(TagCompound tag)
        //{
        //    //tag["exampleLifeFruits"] = exampleLifeFruits;
        //    tag["aghanimShard"] = aghanimShard;
        //}

        //public override void LoadData(TagCompound tag)
        //{
        //    //exampleLifeFruits = tag.GetInt("exampleLifeFruits");
        //    aghanimShard = tag.GetInt("aghanimShard");
        //}
    }
}
