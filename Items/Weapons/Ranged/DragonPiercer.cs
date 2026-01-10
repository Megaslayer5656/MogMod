using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class DragonPiercer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public const int HoldoutDistance = 20;
        public const int MaxCharge = 3;
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 96;
            Item.damage = 36;
            Item.knockBack = 3f;
            Item.shootSpeed = 15f;
            Item.useTime = Item.useAnimation = 30;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item20;
            Item.shoot = ModContent.ProjectileType<DragonPiercerHoldout>();
            Item.useAmmo = AmmoID.Arrow;
        }
        // TODO: give a right click
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 26;
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.autoReuse = false;
                Item.channel = false;
                Item.shoot = ModContent.ProjectileType<DragonPiercerChargeHold>();
            }
            else
            {
                Item.autoReuse = true;
                Item.channel = true;
                Item.shoot = ModContent.ProjectileType<DragonPiercerHoldout>();
            }
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }

        // Spawning the holdout cannot consume ammo
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.ItemTimeIsZero)
                {
                    return false;
                }
            }
            return player.ownedProjectileCounts[Item.shoot] > 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // for some reason it wont let you channel right click, so i gotta fix that
            if (player.altFunctionUse == 2)
            {
                Vector2 shootVelocity = velocity;
                Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);

                Projectile.NewProjectile(source, position, shootDirection, ModContent.ProjectileType<DragonPiercerChargeHold>(), damage, knockback, player.whoAmI);
                // stop player from moving while shooting
                player.AddBuff(ModContent.BuffType<DragonPiercerShot>(), 120);
                return false;
            }
            else
            {
                Vector2 shootVelocity = velocity;
                Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
                // Charge-up. Done via a holdout projectile.
                Projectile.NewProjectile(source, position, shootDirection, ModContent.ProjectileType<DragonPiercerHoldout>(), damage, knockback, player.whoAmI);
                return false;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WindrunnersBow>(1).
                AddIngredient(ItemID.MagicQuiver, 1).
                AddIngredient(ItemID.Cog, 48).
                AddRecipeGroup("AdamantiteBar", 18).
                AddIngredient(ItemID.WirePipe, 8).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}