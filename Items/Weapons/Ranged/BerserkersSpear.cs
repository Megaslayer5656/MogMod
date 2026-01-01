using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    //143x85
    public class BerserkersSpear : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 75;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 100;
            Item.height = 19;
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
            Item.shoot = ModContent.ProjectileType<BerserkersSpearProj>();
            Item.shootSpeed = 12.5f;
            Item.noMelee = true;
        }

        public override bool CanUseItem(Player player)
        {
            float percentLifeLeft = (float)player.statLife / player.statLifeMax2;
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ModContent.ProjectileType<BerserkersFireSpearProj>();
                Item.useTime = Convert.ToInt32(50 * (percentLifeLeft + .1));
                Item.useAnimation = Convert.ToInt32(50 * (percentLifeLeft + .1));
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<BerserkersSpearProj>();
                Item.useTime = 45;
                Item.useAnimation = 45;
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float percentLifeLeft = (float)player.statLife / player.statLifeMax2; 
            if (player.altFunctionUse == 2)
            {
                player.Hurt(PlayerDeathReason.ByCustomReason($"{player.name} sacrificed their lifeblood to the Berserker's Spear"), Convert.ToInt32(player.statLifeMax2 * .04), 0, false, true, 0, false, 1000, 100, 0f);
                type = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<BerserkersFireSpearProj>(), Convert.ToInt32(75 / (percentLifeLeft + .1f)), 2f, player.whoAmI);
                return false;
            }
            else
            {
                return true;
            }
        }


        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WarriorsSpear>(1).
                AddIngredient(ItemID.ShadowFlameKnife, 1).
                AddRecipeGroup("Ichor", 15).
                AddIngredient(ItemID.Ectoplasm, 10).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
