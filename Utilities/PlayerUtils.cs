using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Utilities
{
    public static class PlayerUtils
    {
        public static void SendPacket(this Player player, ModPacket packet, bool server) //Thank you for this idea Calamity Mod, we love you!
        {
            // Client: Send the packet only to the host.
            if (!server)
                packet.Send();

            // Server: Send the packet to every OTHER client.
            else
                packet.Send(-1, player.whoAmI);
        }
        /// <summary>
        /// Calculates and returns the player's total melee scale boosts. This is used mostly for melee holdouts.
        /// </summary>
        /// <param name="addHeldItemScale">If the item scale of the players held item should be added to the calculation.<br/>
        /// Should be disabled for anything that doesn't lock you into holding one item.
        /// </param>
        public static float GetMeleeScale(this Player player, bool addHeldItemScale = true)
        {
            float baseScale = 1;
            player.ApplyMeleeScale(ref baseScale); // Gets vanilla's glove scale boosts
            if (addHeldItemScale)
                baseScale += (player.HeldItem.scale - 1);

            return baseScale;
        }
    }
}
