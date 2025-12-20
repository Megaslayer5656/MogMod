using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Projectiles;
using System;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons
{
    public class Yasha : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 40;
            Item.damage = 70;
            Item.knockBack = 5;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.consumable = false;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.value = Item.buyPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.shootSpeed = 8f;
            Item.shoot = ModContent.ProjectileType<YashaProjectile>();
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 18;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Javelin, 25).
                AddIngredient(ItemID.HallowedBar, 15).
                AddIngredient(ItemID.SoulofSight, 7).
                AddIngredient(ItemID.SoulofFlight, 7).
                AddIngredient<CraftingRecipe>(1).
                AddTile(ItemID.MythrilAnvil).
                Register();
        }
    }
}