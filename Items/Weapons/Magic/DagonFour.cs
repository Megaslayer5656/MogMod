using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class DagonFour : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public const int MaxShots = 2;
        public static Color WeakColor => new(255, 98, 46);
        public static Color StrongColor => new(255, 31, 31);
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxShots);
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;

            Item.damage = 95;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 32;
            Item.useTime = Item.useAnimation = 36;
            Item.knockBack = 2f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<DagonFourHoldout>();
            Item.shootSpeed = 6f;

            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DagonThree>().
                AddIngredient<HellfireBar>(10).
                AddIngredient<ScorchedCore>().
                AddIngredient<UltimateOrb>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}