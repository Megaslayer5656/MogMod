using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class Sharpshooter : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public const int HoldoutDistance = 20;
        public const float MaxCharge = 100f;
        public static bool Empowered = false;
        public static int Charges;
        public override void SetStaticDefaults() => ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 24;
            Item.damage = 53;
            Item.knockBack = 3f;
            Item.shootSpeed = 15f;
            Item.useTime = Item.useAnimation = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = false;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = MogGlobalItem.RarityLightPurpleBuyPrice;
            Item.UseSound = SoundID.Item20;
            Item.shoot = ModContent.ProjectileType<SharpshooterHoldout>();
            Item.useAmmo = AmmoID.Arrow;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool? CanAutoReuseItem(Player player) => player.altFunctionUse == 2;
        public override Vector2? HoldoutOffset() => new Vector2(-3.95f, 0);
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                Item.UseSound = SoundID.Item20;
                Item.noUseGraphic = true;
                Item.channel = true;
                return player.ownedProjectileCounts[Item.shoot] <= 0;
            }
            else
            {
                Item.UseSound = SoundID.Item5;
                Item.noUseGraphic = false;
                Item.channel = false;
                return true;
            }
        }
        // Spawning the holdout cannot consume ammo
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (player.altFunctionUse == 2)
                return true;
            return player.ownedProjectileCounts[Item.shoot] > 0;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Charges--;
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SquirrelProj>(), Empowered ? damage * 4: damage, knockback, player.whoAmI, Empowered ? 1f : 0f);
                if (Charges == 0)
                    Empowered = false;
            }
            else
            {
                Vector2 shootVelocity = velocity;
                Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
                Projectile.NewProjectile(source, position, shootDirection, ModContent.ProjectileType<SharpshooterHoldout>(), damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Pearlwood, 50).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Squirrel"}", 10).
                AddIngredient(ItemID.SoulofSight, 7).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}