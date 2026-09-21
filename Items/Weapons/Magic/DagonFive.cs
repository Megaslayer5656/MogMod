using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class DagonFive : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public const int MaxShots = 3;
        public static Color WeakColor => new(255, 31, 31);
        public static Color StrongColor => new (255, 26, 83);
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxShots);
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;

            Item.damage = 100;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 38;
            Item.useTime = Item.useAnimation = 40;
            Item.knockBack = 8f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<DagonFiveHoldout>();
            Item.shootSpeed = 6f;

            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DagonFour>().
                AddIngredient(ItemID.FragmentSolar, 12).
                AddIngredient<BrokenHeroStaff>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}