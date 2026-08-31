using Microsoft.Xna.Framework;
using MogMod.Common.Config;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class AXMC : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public const int maxShots = 5;
        public const int reloadTime = 240;
        public const int BloodDamage = 200;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(maxShots);
        public override void SetDefaults()
        {
            Item.width = 194;
            Item.height = 38;

            Item.damage = 1641; // enough to one shot eye of junkmunthulugh
            Item.crit = 71;
            Item.knockBack = 14f;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = Item.useAnimation = 100;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<AXMCHoldout>();
            Item.shootSpeed = 3f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;

            MogGlobalItem mogItem = Item.MogMod();
            mogItem.visualBloodDamage = BloodDamage;
        }
        public override bool RangedPrefix() => true;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
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
        public override Vector2? HoldoutOffset() => new Vector2(23.5f, 0.2f);
        public override bool AltFunctionUse(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.scope = true;
                return true;
            }
            return base.AltFunctionUse(player);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Mosin>().
                AddIngredient(ItemID.SniperRifle).
                AddIngredient(ItemID.VortexBeater).
                AddIngredient(ItemID.LunarBar, 12).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}