using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class TheMarker : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 68;
            Item.height = 68;
            Item.damage = 250;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.knockBack = 13f;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.scale = 2f;
            Item.shootSpeed = 10f;
            Item.shoot = ProjectileID.PurificationPowder; //This (and the shoot method) just make the weapon be able to face the direction of your mouse when you swing
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override void AddRecipes() //I'll make this recipe cooler later when we have more on tier crafting recipes.
        {
            CreateRecipe().
            AddIngredient(ItemID.HallowedBar, 15).
            AddIngredient(ModContent.ItemType<LizhardBloodVial>()).
            AddTile(TileID.MythrilAnvil).
            Register();
        }
    }
}