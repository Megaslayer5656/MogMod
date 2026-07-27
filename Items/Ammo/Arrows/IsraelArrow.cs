using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Items.Placeable.Bars;

namespace MogMod.Items.Ammo.Arrows
{
    public class IsraelArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 48;

            Item.damage = 17;
            Item.knockBack = 4f;
            Item.DamageType = DamageClass.Ranged;

            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;

            Item.value = Item.sellPrice(copper: 1);
            Item.rare = ItemRarityID.Yellow;

            Item.ammo = AmmoID.Arrow;
            Item.shoot = ModContent.ProjectileType<IsraelArrowProj>();
            Item.shootSpeed = 5f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(271).
                AddIngredient<FaeBar>(1).
                AddIngredient(ItemID.GoldCoin, 49).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}