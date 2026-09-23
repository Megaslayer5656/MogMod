using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria;
using MogMod.Projectiles.MagicProjectiles;

namespace MogMod.Items.Weapons.Magic.DeckCards
{
    public class VolatileExplosionCard : DeckCard
    {
        public override void SetStaticDefaults()
        {
            cardName = "Volatile Explosion";
            description = "Violently explode.";
            cardMana = 40;
            enabled = true; //Get rid of this when I add way to enable or disable cards in the future.
        }
        public override void doEffect(Player player)
        {
            Projectile i = Projectile.NewProjectileDirect(player.GetSource_Misc("Card Proj"), player.Center, Vector2.Zero, ModContent.ProjectileType<DagonExplosion>(), 150, 8f, player.whoAmI);
            i.DamageType = DamageClass.Magic;
        }
    }
}