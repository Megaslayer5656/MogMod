using Microsoft.Xna.Framework;
using MogMod.Projectiles.RangedProjectiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class WarriorsSpear : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 20;
            Item.height = 4;
            Item.scale = .15f;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.buyPrice(0, 32, 82, 5);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<WarriorsSpearProj>();
            Item.shootSpeed = 12.5f;
            Item.noMelee = true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ModContent.ProjectileType<WarriorsFireSpearProj>();
                Item.useTime = 42;
                Item.useAnimation = 42;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<WarriorsSpearProj>();
                Item.useTime = 60;
                Item.useAnimation = 60;
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.Hurt(PlayerDeathReason.ByCustomReason($"{player.name} sacrificed their lifeblood to the Warrior's Spear"), Convert.ToInt32(player.statLifeMax2 * .04), 0, false, true, 0, false, 1000, 100, 0f);
                type = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<WarriorsFireSpearProj>(), 50, 2f, player.whoAmI);
                return false;
            } else
            {
                return true;
            }
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Torch, 30).
                AddIngredient(ItemID.Javelin, 15). // change to recipe group maybe??
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Bar"}", 12).
                AddTile(TileID.Anvils).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.Torch, 30).
                AddIngredient(ItemID.BoneJavelin, 15).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Bar"}", 12).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
