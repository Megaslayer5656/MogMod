using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
namespace MogMod.Items
{
    public class BouncerFury : ModItem
    {
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.MogMod.hjson file.

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Melee;
            Item.width = 80;
            Item.height = 80;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = 1;
            Item.knockBack = 20f;
            Item.value = 10000;
            Item.rare = 5;
            Item.UseSound = SoundID.Item70;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.BouncyBoulder;
            Item.shootSpeed = 30f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float PosX = Main.MouseWorld.X; //Makes the projectile always spawn above the cursor
            float PosY = player.position.Y - 600f; //makes the projectile spawn in the sky so it can shoot down
            Projectile.NewProjectile(source, PosX, PosY, 0f, 1f, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod, "BoulderFury");
            recipe.AddIngredient(ItemID.BouncyBoulder, 50);
            recipe.AddIngredient(ItemID.SoulofMight, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}