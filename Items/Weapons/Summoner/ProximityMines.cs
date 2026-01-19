using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.SummonerProjectiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Summoner
{
    public class ProximityMines : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summoner";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
            Item.mana = 10;
            Item.width = Item.height = 50;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<ProximityMinesSummon>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.FindSentryRestingSpot(type, out int XPosition, out int YPosition, out int YOffset);
            YOffset -= 6;
            position = new Vector2((float)XPosition, (float)(YPosition - YOffset));
            int p = Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, 120f, 0f);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpikyBall, 100).
                AddIngredient(ItemID.Dynamite, 15).
                AddIngredient(ItemID.Hellstone, 10).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
