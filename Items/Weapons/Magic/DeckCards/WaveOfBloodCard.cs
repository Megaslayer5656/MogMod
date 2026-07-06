using System;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using MogMod.Projectiles.Melee;

namespace MogMod.Items.Weapons.Magic.DeckCards
{
    public class WaveOfBloodCard : DeckCard
    {
        public override void SetStaticDefaults()
        {
            cardName = "Wave of Blood";
            description = "Shoots a wave of blood";
            cardMana = 50;
            enabled = true; //Get rid of this when I add way to enable or disable cards in the future.
        }

        public override void doEffect(Player player)
        {
            float speed = 12f;
            Vector2 velocity = Main.MouseWorld - player.Center; //Use this code for misc projectile stuff from these cards.
            velocity.Normalize();
            velocity *= speed;

            Projectile i = Projectile.NewProjectileDirect(player.GetSource_Misc("Card Proj"), player.Center, velocity, ModContent.ProjectileType<RiversOfBloodProj>(), 100, 2f, player.whoAmI);
            i.DamageType = DamageClass.Magic;
        }
    }
}
