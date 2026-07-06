using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace MogMod.Items.Weapons.Magic.DeckCards
{
    public class HealCard : DeckCard
    {
        public override void SetStaticDefaults()
        {
            cardName = "Heal";
            description = "Heals the caster.";
            enabled = true; //Get rid of this when I add way to enable or disable cards in the future.
        }

        public override void doEffect(Player player)
        {
            player.Heal(20);
            if (player.statLife > player.statLifeMax2)
                player.statLife = player.statLifeMax2;

        }
    }
}
