using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.MeleeProjectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class GreatswordOfSouls : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override void SetDefaults()
        {
            Item.width = 86;
            Item.height = 86;
            Item.damage = 175;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = false;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1; //Might change this sound but def not to a sickle sound I feel like that's out of place but we can change this Will
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
            Item.shoot = ModContent.ProjectileType<GreatswordOfSoulsProj>();
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 3;
            float rotation = MathHelper.ToRadians(15);
            position += Vector2.Normalize(velocity) * 45f;
            float PosX = Main.MouseWorld.X;
            float PosY = player.position.Y;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .8f;
                int proj2 = Projectile.NewProjectile(source, position, perturbedSpeed, type, Convert.ToInt32(damage * .625), knockback, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GriefBar>(12).
                AddIngredient<SoulFragment>(8).
                AddIngredient(ItemID.SoulofNight, 7).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
