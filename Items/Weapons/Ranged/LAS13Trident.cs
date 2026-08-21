using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class LAS13Trident : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public static int ArmorPenetration = 15;
        public int BuiltUpHeat = 0;
        public const int OverheatLevel = 360;
        public const int OverheatCooldown = 180;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ArmorPenetration, OverheatCooldown.FramesToSeconds());
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 24;

            Item.damage = 24;
            Item.knockBack = 2.5f;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useAmmo = AmmoID.Gel;
            Item.shoot = ModContent.ProjectileType<LAS13Holdout>();
            Item.shootSpeed = 3f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
        }
        public override void HoldItem(Player player)
        {
            player.MogMod().rightClickListener = true;
            if (player.ownedProjectileCounts[Item.shoot] > 0 && !Main.mouseLeft && BuiltUpHeat > 0 && player.MogMod().lasOverheat == 0)
            {
                BuiltUpHeat -= 3;
                if (BuiltUpHeat < 0) BuiltUpHeat = 0;
            }
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
        public override Vector2? HoldoutOffset() => new Vector2(-5f, 0f);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyAdamantiteBar", 13).
                AddIngredient(ItemID.SoulofMight, 7).
               AddIngredient(ItemID.IllegalGunParts).
                AddIngredient(ItemID.GolfCupFlagBlue).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}