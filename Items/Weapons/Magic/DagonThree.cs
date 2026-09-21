using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class DagonThree : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public static Color WeakColor => new(255, 139, 61);
        public static Color StrongColor => new(255, 98, 46);
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;

            Item.damage = 154;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 22;
            Item.useTime = Item.useAnimation = 30;
            Item.knockBack = 2f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<DagonThreeHoldout>();
            Item.shootSpeed = 6f;

            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DagonTwo>().
                AddIngredient(ItemID.HellstoneBar, 8).
                AddIngredient<PointBooster>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}