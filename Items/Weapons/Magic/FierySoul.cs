using Microsoft.Xna.Framework;
using MogMod.Buffs;
using MogMod.Common.Player;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class FierySoul : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.damage = 28;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Magic;
            Item.useAnimation = Item.useTime = 24;
            Item.mana = 12;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.shootSpeed = 15f;
            Item.shoot = ModContent.ProjectileType<FierySoulProjectile>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item20;
            Item.rare = ItemRarityID.Orange;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Book, 1).
                AddIngredient(ItemID.FlowerofFire, 1).
                AddIngredient(ItemID.Fireblossom, 3).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}