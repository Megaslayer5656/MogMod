using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo
{
    public class WaveBullet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 20;

            Item.damage = 12;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Ranged;

            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;

            Item.value = Item.sellPrice(copper: 12);
            Item.rare = ItemRarityID.LightPurple;

            Item.shoot = ModContent.ProjectileType<WaveProj>();
            Item.shootSpeed = 0f;

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