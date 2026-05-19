using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using MogMod.Projectiles.RangedProjectiles;

namespace MogMod.Items.Ammo
{
    public class BouncyBoulderBullet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.DamageType = DamageClass.Ranged;
            Item.width = Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 7f;
            Item.value = Item.sellPrice(copper: 30);
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<BouncyBoulderBulletProj>();
            Item.shootSpeed = 2f;
            Item.ammo = ItemID.MusketBall;
        }
        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient<BoulderBullet>(100).
                AddIngredient(ItemID.PinkGel, 5).
                AddTile(TileID.Anvils).
                Register();
            CreateRecipe(100).
                AddIngredient(ItemID.MusketBall, 100).
                AddIngredient(ItemID.BouncyBoulder).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}