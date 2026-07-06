using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;

namespace MogMod.Items.Weapons.Magic.DeckCards
{
    public class SeethingSongCard : DeckCard
    {
        public override void SetStaticDefaults()
        {
            cardName = "Seething Song";
            description = "Lights the caster ablaze, but restores some mana";
            cardMana = 0;
            enabled = true; //Get rid of this when I add way to enable or disable cards in the future.
        }

        public override void doEffect(Player player)
        {
            player.AddBuff(BuffID.OnFire, 180);

            Player.HurtInfo info = new Player.HurtInfo();
            info.Damage = 20;
            info.Knockback = 0f;
            info.HitDirection = player.direction;
            info.DamageSource = PlayerDeathReason.ByCustomReason(player.name + " was burned by Seething Song.");

            player.Hurt(info);
            if (player.statLife > player.statLifeMax2)
                player.statLife = player.statLifeMax2;

            player.statMana += 50;
            if (player.statMana > player.statManaMax2)
                player.statMana = player.statManaMax2;

            player.ManaEffect(50);

        }
    }
}
