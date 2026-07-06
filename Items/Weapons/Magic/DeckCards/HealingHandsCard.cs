using Terraria;

namespace MogMod.Items.Weapons.Magic.DeckCards
{
    public class HealingHandsCard : DeckCard
    {
        public override void SetStaticDefaults()
        {
            cardName = "Healing Hands";
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
