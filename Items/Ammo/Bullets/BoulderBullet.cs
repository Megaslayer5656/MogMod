using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using MogMod.Projectiles.RangedProjectiles;

namespace MogMod.Items.Ammo.Bullets
{
    public class BoulderBullet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetDefaults()
        {
            Item.damage = 7; //This was WAY too broken
            Item.DamageType = DamageClass.Ranged;
            Item.width = Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 7f;
            Item.value = Item.sellPrice(copper: 20);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<BoulderBulletProj>();
            Item.shootSpeed = 2f;
            Item.ammo = ItemID.MusketBall;
        }
        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient(ItemID.MusketBall, 100).
                AddIngredient(ItemID.Boulder, 1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}