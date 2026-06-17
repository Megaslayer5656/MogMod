using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MeleeProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    // TODO: resprite this (fextralife logo is plastered on the weapon)
    public class BlackBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public const float MaxCharge = 100f;
        public override void SetDefaults()
        {
            Item.width = 97;
            Item.height = 96;

            Item.damage = 255;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 80;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 14f;
            Item.UseSound = SoundID.Item20 with { Pitch = -0.15f };
            Item.shoot = ModContent.ProjectileType<BlackBladeHoldout>();
            Item.shootSpeed = 10f;

            Item.channel = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectileDirect(source, player.MountedCenter, Vector2.Zero, type, damage, knockback, player.whoAmI, 0);
            return false;
        }
        public override bool? CanAutoReuseItem(Player player) => false;
        public override bool MeleePrefix() => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BreakerBlade, 1).
                AddIngredient(ItemID.ShadowFlameKnife, 1).
                AddIngredient(ItemID.HallowedBar, 15).
                AddIngredient<UltimateOrb>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}