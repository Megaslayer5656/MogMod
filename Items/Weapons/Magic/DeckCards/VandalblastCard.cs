using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic.DeckCards
{
    public class VandalblastCard : DeckCard
    {
        public override void SetStaticDefaults()
        {
            cardName = "Vandalblast";
            description = "Shoots a small blast of fire";
            cardMana = 10;
            enabled = true; //Get rid of this when I add way to enable or disable cards in the future.
        }

        public override void doEffect(Player player)
        {
            float speed = 15f;
            Vector2 velocity = Main.MouseWorld - player.Center; //Use this code for misc projectile stuff from these cards.
            velocity.Normalize();
            velocity *= speed;

            Projectile i = Projectile.NewProjectileDirect(player.GetSource_Misc("Card Proj"), player.Center, velocity, ProjectileID.BallofFire, 30, 2f, player.whoAmI);
            i.DamageType = DamageClass.Magic;
        }
    }
}
