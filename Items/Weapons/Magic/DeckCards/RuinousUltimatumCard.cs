using MogMod.Projectiles.Melee;
using MogMod.Utilities;
using System;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace MogMod.Items.Weapons.Magic.DeckCards
{
    public class RuinousUltimatumCard : DeckCard
    {
        public override void SetStaticDefaults()
        {
            cardName = "Ruinous Ultimatum";
            description = "Summons a barrage of flaming arrows from the sky.";
            cardMana = 40;
            enabled = true; //Get rid of this when I add way to enable or disable cards in the future.
        }

        public override void doEffect(Player player)
        {
            float speed = 15f;
            Vector2 velocity = Main.MouseWorld - player.Center; //Use this code for misc projectile stuff from these cards.
            velocity.Normalize();
            velocity *= speed;

            for (int i = 0; i < 15; ++i)
            {
                float randSpeed = speed * Main.rand.NextFloat(0.7f, 1.4f);
                MogModUtils.ProjectileRain(player.GetSource_Misc("Card Proj"), Main.MouseWorld, 300f, 50f, 850f, 1100f, randSpeed, ProjectileID.FireArrow, 30, 2f, player.whoAmI);
            }
        }
    }
}
