using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace MogMod.Items.Weapons.Magic.DeckCards
{
    public class BalrightHealCard : DeckCard
    {
        public override void SetStaticDefaults()
        {
            cardName = "Balright Heal";
            description = "Heals the caster using the power of the Balright monster.";
            enabled = true; //Get rid of this when I add way to enable or disable cards in the future.
        }

        public override void doEffect(Player player)
        {
            player.Heal(200);
            if (player.statLife > player.statLifeMax2)
                player.statLife = player.statLifeMax2;

            player.AddBuff(BuffID.Blackout, 180);
        }
    }
}
