using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Items.Weapons
{
    public class BoulderFury : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Melee;
            Item.width = 80;
            Item.height = 80;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = 1;
            Item.knockBack = 7f;
            Item.value = 10000;
            Item.rare = 4;
            Item.UseSound = SoundID.Item70;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Boulder;
            Item.shootSpeed = 30f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float PosX = Main.MouseWorld.X;
            float PosY = player.position.Y - 600f; 
            int proj = Projectile.NewProjectile(source, PosX, PosY, 0f, 1f, type, damage, knockback, player.whoAmI);
            Main.projectile[proj].friendly = true;
            return false;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Starfury, 1);
            recipe.AddIngredient(ItemID.Boulder, 50);
            recipe.AddIngredient(ItemID.DemoniteBar, 15);
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}