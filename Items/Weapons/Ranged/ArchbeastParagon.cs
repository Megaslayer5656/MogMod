using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class ArchbeastParagon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public const int MaxShots = 5;
        public const double DamageMult = 2;
        public const int MinCharge = 30;
        public const int MaxCharge = 330 - MinCharge;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageMult);
        public override void SetStaticDefaults() => ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 108;

            Item.crit = 28;
            Item.damage = 175;
            Item.knockBack = 5f;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = ModContent.ProjectileType<ArchbeastParagonHoldout>();
            Item.shootSpeed = 3f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override bool RangedPrefix() => true;
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && player.altFunctionUse != 2;
        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI) player.MogMod().rightClickListener = true;
            player.MogMod().mouseWorldListener = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2) return false;
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
                AddIngredient<DragonPiercer>().
                AddIngredient(ItemID.DD2PhoenixBow).
                AddIngredient<UltimateOrb>().
                AddIngredient<BrokenHeroGun>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}