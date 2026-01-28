using Microsoft.Xna.Framework;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class DreadsRetribution : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 72;
            Item.height = 36;
            Item.damage = 100;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 7;
            Item.useAnimation = 21;
            Item.useLimitPerAnimation = 3;
            Item.reuseDelay = Item.useAnimation - 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float tenthPi = 0.314159274f;
            Vector2 arrowVel = velocity;
            arrowVel.Normalize();
            arrowVel *= 50f;
            bool arrowHitsTiles = Collision.CanHit(vector2, 0, 0, vector2 + arrowVel, 0, 0);
            for (int i = 0; i < 2; i++)
            {
                float piOffsetValue = (float)i - .4f;
                Vector2 offsetSpawn = arrowVel.RotatedBy((double)(tenthPi * piOffsetValue), default);
                if (!arrowHitsTiles)
                {
                    offsetSpawn -= arrowVel;
                }
                int arrowSpawn = Projectile.NewProjectile(source, vector2.X + offsetSpawn.X, vector2.Y + offsetSpawn.Y, velocity.X, velocity.Y, ModContent.ProjectileType<DreadsProj>(), damage, knockback, player.whoAmI);
                Main.projectile[arrowSpawn].noDropItem = true;
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DrowRangersCrossbow>(1).
                AddIngredient(ItemID.Tsunami, 1).
                AddIngredient(4953, 1). // eventide from empress
                AddIngredient(ItemID.BeetleHusk, 8).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}