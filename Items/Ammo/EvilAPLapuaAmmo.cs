using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using MogMod.Projectiles.RangedProjectiles;

namespace MogMod.Items.Ammo
{
    public class EvilAPLapuaAmmo : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 9;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(copper: 11);
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<EvilAPLapua>();
            Item.shootSpeed = 3.5f;
            Item.ammo = ItemID.MusketBall;
        }
        public override void AddRecipes()
        {
            CreateRecipe(70).
                AddIngredient(ItemID.MusketBall, 70).
                AddIngredient(ItemID.ShimmerBlock, 1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}