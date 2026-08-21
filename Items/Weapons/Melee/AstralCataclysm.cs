using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    // 92x110
    // charged greatsword that launches out 3-5 stars that come to a stop
    // sword can hit stars again to launch them again and upgrade them into stronger stars
    // think holy collider calamity
    public class AstralCataclysm : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public const float MaxCharge = 100f;
        public static int MinStars = 2;
        public static int MaxStars = 5;
        public override void SetDefaults()
        {
            Item.width = 92;
            Item.height = 110;

            Item.damage = 1620;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 14f;
            Item.shoot = ModContent.ProjectileType<AstralCataclysmHoldout>();
            Item.shootSpeed = 10f;

            Item.useTurn = true;
            Item.channel = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
        }
        public override bool CanShoot(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
        public override bool CanUseItem(Player player) => base.CanUseItem(player);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectileDirect(source, player.MountedCenter, Vector2.Zero, type, damage, knockback, player.whoAmI, ai2: 5f);
            return false;
        }
        public override bool MeleePrefix() => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.StarWrath).
                AddIngredient<BlackBlade>().
                AddIngredient(ItemID.HallowedBar, 15).
                AddIngredient(ItemID.FallenStar, 12).
                AddIngredient(ItemID.LunarBar, 10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}