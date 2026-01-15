using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using MogMod.Projectiles.RangedProjectiles;

namespace MogMod.Items.Ammo
{
    public class ShrapnalBullet : ModItem
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
            Item.value = Item.sellPrice(copper: 24);
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<ShrapnalProj>();
            Item.shootSpeed = 5f;
            Item.ammo = ItemID.MusketBall;
        }
        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient(ItemID.MusketBall, 1000).
                AddIngredient(ItemID.SoulofFlight, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}