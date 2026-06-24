using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using MogMod.Items.Global;
using MogMod.Projectiles.RangedProjectiles;

namespace MogMod.Items.Ammo
{
    public class EndlessGreenTracerPouch : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 26;
            Item.height = 34;
            Item.knockBack = 3.5f;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
            Item.shoot = ModContent.ProjectileType<GreenTracerProj>();
            Item.shootSpeed = 4.5f;
            Item.ammo = AmmoID.Bullet;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GreenTracerAmmo>(3996).
                AddTile(TileID.CrystalBall).
                Register();
        }
    }
}
