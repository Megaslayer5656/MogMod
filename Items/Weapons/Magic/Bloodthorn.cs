using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class Bloodthorn : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.damage = 57;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 23;
            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BloodthornOrb>();
            Item.shootSpeed = 18f;
            Item.rare = ItemRarityID.Yellow;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 v = velocity;
            float offset = 3f;
            Projectile.NewProjectile(source, position, v + offset * Vector2.UnitX, type, damage, knockback, player.whoAmI, 1f);
            Projectile.NewProjectile(source, position, v + (offset + 2) * Vector2.UnitY, type, damage, knockback, player.whoAmI, 1f);
            Projectile.NewProjectile(source, position, v - offset * Vector2.UnitX, type, damage, knockback, player.whoAmI, 1f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<OrchidMalevolence>(1).
                AddIngredient(ItemID.MagnetSphere, 1).
                AddIngredient(ItemID.Stinger, 15).
                AddIngredient(ItemID.DarkShard, 1).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
