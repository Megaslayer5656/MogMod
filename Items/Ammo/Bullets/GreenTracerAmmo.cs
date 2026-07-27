using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using MogMod.Projectiles.RangedProjectiles;

namespace MogMod.Items.Ammo.Bullets
{
    public class GreenTracerAmmo : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 3.5f;
            Item.value = Item.sellPrice(copper: 4);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<GreenTracerProj>();
            Item.shootSpeed = 4.5f;
            Item.ammo = AmmoID.Bullet;
        }
        public override void AddRecipes()
        {
            CreateRecipe(70).
                AddIngredient(ItemID.Gel, 1).
                AddIngredient(ItemID.HellstoneBar, 1).
                AddIngredient(ItemID.MusketBall, 70).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}