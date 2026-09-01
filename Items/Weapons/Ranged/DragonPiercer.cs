using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class DragonPiercer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public const int MaxShots = 5;
        public const int MinCharge = 30;
        public const int MaxCharge = 240 - MinCharge;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxShots);
        public override void SetStaticDefaults() => ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 96;

            Item.crit = 14;
            Item.damage = 140;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = ModContent.ProjectileType<DragonPiercerHoldout>();
            Item.shootSpeed = 3f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.LightPurple;
            Item.value = MogGlobalItem.RarityLightPurpleBuyPrice;
        }
        public override bool RangedPrefix() => true;
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI) player.MogMod().rightClickListener = true;
            player.MogMod().mouseWorldListener = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile holdout = Projectile.NewProjectileDirect(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI);
            holdout.velocity = (player.MogMod().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero);
            return false;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) position += muzzleOffset;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WindrunnersBow>().
                AddIngredient(ItemID.MagicQuiver).
                AddIngredient(ItemID.Cog, 48).
                AddRecipeGroup("AnyAdamantiteBar", 18).
                AddIngredient<FuciumBar>(8).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}