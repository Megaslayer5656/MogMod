using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class DagonTwo : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public static Color WeakColor => new(255, 188, 105);
        public static Color StrongColor => new(255, 139, 61);
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;

            Item.damage = 56;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 2f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<DagonTwoHoldout>();
            Item.shootSpeed = 2f;

            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DagonOne>().
                AddIngredient(ItemID.MeteoriteBar, 12).
                AddIngredient(ItemID.Fireblossom, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}