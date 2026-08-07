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
    public class BlackBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public const float MaxCharge = 100f;
        public override void SetDefaults()
        {
            Item.width = Item.height = 114;



            Item.damage = 1120;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 14f;
            Item.shoot = ModContent.ProjectileType<BlackBladeHoldout>();
            Item.shootSpeed = 10f;

            Item.useTurn = true;
            Item.channel = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
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
                AddIngredient(ItemID.BreakerBlade, 1).
                AddIngredient<WyvernJawblade>().
                AddIngredient<SpookyEssence>(20).
                AddIngredient(ItemID.HallowedBar, 15).
                AddIngredient<UltimateOrb>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}