using Microsoft.Xna.Framework;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class DrowRangersCrossbow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 28;
            Item.damage = 72;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 7;
            Item.useAnimation = 21;
            Item.reuseDelay = Item.useAnimation - 4;
            Item.useLimitPerAnimation = 3;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Arrow;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int arrowSpawn = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<DrowRangerArrow>(), damage, knockback, player.whoAmI);
            Main.projectile[arrowSpawn].noDropItem = true;
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WindrunnersBow>(1).
                AddIngredient(ItemID.HallowedRepeater).
                AddIngredient(ItemID.SoulofNight, 8).
                AddIngredient(ItemID.FrostCore, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}