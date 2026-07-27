using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Items.Placeable.Bars;

namespace MogMod.Items.Ammo.Arrows
{
    public class RuntyArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 32;

            Item.damage = 4;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Ranged;

            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;

            Item.value = Item.sellPrice(copper: 2);
            Item.rare = ItemRarityID.Blue;

            Item.ammo = AmmoID.Arrow;
            Item.shoot = ModContent.ProjectileType<RuntyArrowProj>();
            Item.shootSpeed = 3f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(50).
                AddIngredient(ItemID.WoodenArrow, 50).
                AddIngredient<RuntyBar>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
}