using System;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic.DeckCards
{
    public class SmotheringTitheCard : DeckCard
    {
        public override void SetStaticDefaults()
        {
            cardName = "Smothering Tithe";
            description = "Shoots a barrage of gold coins.";
            cardMana = 40;
            enabled = true; //Get rid of this when I add way to enable or disable cards in the future.
        }

        public override void doEffect(Player player)
        {
            float speed = 10f;

            Vector2 direction = Main.MouseWorld - player.Center;
            direction.Normalize();

            Vector2 perpendicular = direction.RotatedBy(MathHelper.PiOver2);

            for (int i = 0; i < 5; i++)
            {
                Vector2 spawnPos = player.Center - direction * Main.rand.NextFloat(20f, 40f) + perpendicular * Main.rand.NextFloat(-15f, 15f);
                float spread = MathHelper.ToRadians(Main.rand.NextFloat(-7f, 7f));
                Vector2 vel = direction.RotatedBy(spread);
                vel *= speed * Main.rand.NextFloat(0.85f, 1.15f);

                Projectile k = Projectile.NewProjectileDirect(player.GetSource_Misc("Card Proj"), spawnPos, vel, ProjectileID.GoldCoin, 30, 2f, player.whoAmI);
                k.DamageType = DamageClass.Magic;
            }
        }
    }
}
