using Terraria;
using Terraria.DataStructures;

namespace MogMod.Items.Weapons.Magic.DeckCards
{
    public class DarkRitualCard : DeckCard
    {
        public override void SetStaticDefaults()
        {
            cardName = "Dark Ritual";
            description = "Damages the caster, but restores some mana.";
            cardMana = 10;
            enabled = true; //Get rid of this when I add way to enable or disable cards in the future.
        }

        public override void doEffect(Player player)
        {
            Player.HurtInfo info = new Player.HurtInfo();
            info.Damage = 20;
            info.Knockback = 0f;
            info.HitDirection = player.direction;
            info.DamageSource = PlayerDeathReason.ByCustomReason(player.name + " gave their life to the dark ritual.");

            player.Hurt(info);
            if (player.statLife > player.statLifeMax2)
                player.statLife = player.statLifeMax2;
            player.immune = false;
            player.immuneTime = 0;

            player.statMana += 50;
            if (player.statMana > player.statManaMax2)
                player.statMana = player.statManaMax2;

            player.ManaEffect(50);

        }
    }
}
