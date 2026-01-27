using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Items.Consumables;

namespace MogMod.Items.Ammo
{
    public class AghanimBulletAmmo : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 10;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 4f;
            Item.value = Item.sellPrice(copper: 100);
            Item.rare = ItemRarityID.Purple;
            Item.shoot = ModContent.ProjectileType<AghanimBulletProj>();
            Item.shootSpeed = 7f;
            Item.ammo = ItemID.MusketBall;
        }
        public override void AddRecipes()
        {
            CreateRecipe(999).
                AddIngredient(ItemID.MoonlordBullet, 999).
                AddIngredient<AghanimShard>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}