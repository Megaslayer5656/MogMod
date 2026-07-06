using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using MogMod.Projectiles.Melee;
using MogMod.Utilities;
using Mono.Cecil;
using static System.Net.Mime.MediaTypeNames;

namespace MogMod.Items.Weapons.Magic.DeckCards
{
    public class VandalblastOverloadCard : DeckCard
    {
        public override void SetStaticDefaults()
        {
            cardName = "Vandalblast Overload";
            description = "Summons fireballs from the sky.";
            cardMana = 50;
            enabled = true; //Get rid of this when I add way to enable or disable cards in the future.
        }

        public override void doEffect(Player player)
        {
            float speed = 10f;
            Vector2 velocity = Main.MouseWorld - player.Center; //Use this code for misc projectile stuff from these cards.
            velocity.Normalize();
            velocity *= speed;

            int meteorProj = ModContent.ProjectileType<FlameMeteorProj>();
            for (int i = 0; i < 6; ++i)
            {
                float randSpeed = speed * Main.rand.NextFloat(0.7f, 1.4f);
                MogModUtils.ProjectileRain(player.GetSource_Misc("Card Proj"), Main.MouseWorld, 300f, 50f, 850f, 1100f, randSpeed, meteorProj, 50, 2f, player.whoAmI);
            }
        }
    }
}
