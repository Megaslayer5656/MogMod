using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic.DeckCards
{
    public abstract class DeckCard : ModSystem
    {
        public static bool enabled = false;
        public string cardName = "";
        public string description = "";
        public int cardMana = 20;
        public bool unlocked = false;
        public string getCardName()
        {
            return cardName;
        }
        public bool getEnabled()
        {
            return enabled;
        }
        public virtual void doEffect(Player player)
        {

        }
        public static void enableCard()
        {
            enabled = true;
        }
    }
}
