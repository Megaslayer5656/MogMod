using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Accessories;
using MogMod.Items.Other;
using MogMod.Projectiles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class BattleFury : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 32;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.shootSpeed = 12f;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.buyPrice(0, 38, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.shoot = ModContent.ProjectileType<BattleFuryProjectile>();
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MoltenHamaxe, 1).
                AddRecipeGroup("IronBar", 24).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}