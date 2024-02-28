using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Items
{
    public class BoulderGun : ModItem
    {
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.MogMod.hjson file.

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 15;
            Item.height = 15;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = 5;
            Item.knockBack = 2.5f;
            Item.value = 10000;
            Item.rare = 3;
            Item.UseSound = SoundID.Item7;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Boulder;
            Item.shootSpeed = 30f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float PosX = player.position.X - 30f; //Makes the projectile always spawn above the cursor
            float PosY = player.position.Y - 0f; //makes the projectile spawn in the sky so it can shoot down
            Projectile.NewProjectile(source, PosX, PosY, 30f, 1f, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}