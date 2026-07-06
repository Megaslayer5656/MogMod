using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic.DeckCards
{
    public class ShockCard : DeckCard
    {
        public override void SetStaticDefaults()
        {
            cardName = "Shock";
            description = "A near instant red lighting blast";
            cardMana = 10;
            enabled = true; //Get rid of this when I add way to enable or disable cards in the future.
        }
        
        public override void doEffect(Player player)
        {
            float speed = 35f;
            Vector2 velocity = Main.MouseWorld - player.Center; //Use this code for misc projectile stuff from these cards.
            velocity.Normalize();
            velocity *= speed;

            Projectile i = Projectile.NewProjectileDirect(player.GetSource_Misc("Card Proj"), player.Center, velocity, ProjectileID.MiniRetinaLaser, 20, 2f, player.whoAmI);
            i.DamageType = DamageClass.Magic;
        }
    }
}
