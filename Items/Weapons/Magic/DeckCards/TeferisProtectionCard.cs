using System;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace MogMod.Items.Weapons.Magic.DeckCards
{
    public class TeferisProtectionCard : DeckCard
    {
        public override void SetStaticDefaults()
        {
            cardName = "Teferi's Protection";
            description = "Phases the caster out of reality";
            cardMana = 30;
            enabled = true; //Get rid of this when I add way to enable or disable cards in the future.
        }

        public override void doEffect(Player player)
        {
            player.SetImmuneTimeForAllTypes(180);

            for (int i = 0; i < player.hurtCooldowns.Length; i++)
            {
                player.hurtCooldowns[i] = 180;
            }
        }
    }
}
