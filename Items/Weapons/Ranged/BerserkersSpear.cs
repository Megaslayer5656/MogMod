using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.RangedProjectiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    //143x85 21 to 45
    public class BerserkersSpear : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        // lets you repeatedly right click
        public override void SetStaticDefaults() => ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        public override void SetDefaults()
        {
            Item.damage = 75;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 20;
            Item.height = 4; // so it doesnt hit the floor when you fire
            Item.scale = .15f;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.shoot = ModContent.ProjectileType<BerserkersSpearProj>();
            Item.shootSpeed = 15f;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                float percentLifeLeft = (float)player.statLife / player.statLifeMax2;
                Item.useTime = Convert.ToInt32(50 * (percentLifeLeft + .1));
                Item.useAnimation = Convert.ToInt32(50 * (percentLifeLeft + .1));
                return true;
            }
            Item.useTime = 45;
            Item.useAnimation = 45;
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                float percentLifeLeft = (float)player.statLife / player.statLifeMax2;
                // hurts the player and ignores i-frames
                player.Hurt(PlayerDeathReason.ByCustomReason($"{player.name} sacrificed their lifeblood to the Berserker's Spear"), Convert.ToInt32(player.statLifeMax2 * .04), -player.direction, false, false, -1, false, 1000, 0, 0);
                player.immune = false;
                player.immuneTime = 0;
                damage = (int)(Item.damage / (percentLifeLeft + .3f));
                type = ModContent.ProjectileType<BerserkersFireSpearProj>();
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback * 2f, player.whoAmI);
                return false;
            }
            return true;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WarriorsSpear>(1).
                AddIngredient(ItemID.ShadowFlameKnife, 1).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Hardmode Evil Material"}", 15).
                AddIngredient<GriefBar>(10).
                AddIngredient(ItemID.Ectoplasm, 7).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}