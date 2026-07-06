using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Items.Placeable.Bars;

namespace MogMod.Items.Ammo
{
    public class EnchantedArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 32;

            Item.damage = 15;
            Item.knockBack = 5f;
            Item.DamageType = DamageClass.Ranged;

            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;

            Item.value = Item.sellPrice(copper: 40);
            Item.rare = ItemRarityID.Yellow;

            Item.ammo = AmmoID.Arrow;
            Item.shoot = ModContent.ProjectileType<EnchantedArrowProj>();
            Item.shootSpeed = 2f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(750).
                AddIngredient(ItemID.JestersArrow, 750).
                AddIngredient<FaeBar>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}