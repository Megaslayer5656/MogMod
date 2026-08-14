using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class HellfireMaxigun : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public int BuiltUpHeat = 0;
        public const int OverheatLevel = 540;
        public const int OverheatCooldown = 180;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(OverheatCooldown.FramesToSeconds());
        public override void SetDefaults()
        {
            Item.width = 66;
            Item.height = 30;

            Item.damage = 58;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;

            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<HellfireMaxigunHoldout>();
            Item.shootSpeed = 3f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
        }
        public override void HoldItem(Player player)
        {
            player.MogMod().rightClickListener = true;
            // Heat decrements if:
            // The holdout exists, the player is not firing, the weapon is not overheating, and there is heat in the weapon
            if (player.ownedProjectileCounts[Item.shoot] > 0 && !Main.mouseLeft && BuiltUpHeat > 0 && player.MogMod().hellfireOverheat == 0)
            {
                BuiltUpHeat -= 3;
                if (BuiltUpHeat < 0) BuiltUpHeat = 0;
            }
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override bool RangedPrefix() => true;
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
        public override void AddRecipes() // adamantite tier, pre-mech
        {
            CreateRecipe().
               AddIngredient(ItemID.Gatligator).
               AddIngredient<HellfireBar>(12).
               AddIngredient<ScorchedCore>().
               AddTile(TileID.MythrilAnvil).
               Register();
        }
    }
}