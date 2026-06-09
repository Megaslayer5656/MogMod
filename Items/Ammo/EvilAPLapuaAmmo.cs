using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo
{
    public class EvilAPLapuaAmmo : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetDefaults()
        {
            Item.damage = 11;
            Item.DamageType = DamageClass.Ranged;
            Item.width = Item.height = 8;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(copper: 22);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<EvilAPLapua>();
            Item.shootSpeed = 5f;
            Item.ammo = ItemID.MusketBall;
        }
        public override void AddRecipes()
        {
            CreateRecipe(999).
                AddIngredient(ItemID.ChlorophyteBullet, 999).
                AddIngredient<LizhardBloodVial>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}