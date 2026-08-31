using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
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
    public class DreadsRetribution : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public const int maxShots = 3;
        public const int reloadTime = 60;
        public const int ArmorPenetration = 20;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(maxShots * 2, reloadTime.FramesToSeconds(), ArmorPenetration);
        public override void SetDefaults()
        {
            Item.width = 72;
            Item.height = 36;

            Item.damage = 140;
            Item.knockBack = 3f;
            Item.ArmorPenetration = ArmorPenetration;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = ModContent.ProjectileType<DreadsRetributionHoldout>();
            Item.shootSpeed = 3f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
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
        public override Vector2? HoldoutOffset() => new Vector2(-10f, 0f);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DrowRangersCrossbow>(1).
                AddIngredient(ItemID.Tsunami, 1).
                AddIngredient(ItemID.FairyQueenRangedItem).
                AddIngredient<GriefBar>(5).
                AddIngredient<FrigidCrystal>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}