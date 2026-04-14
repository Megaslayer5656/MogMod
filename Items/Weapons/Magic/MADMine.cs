using Microsoft.Xna.Framework;
using MogMod.Items.Consumables;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class MADMine : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 32;
            
            Item.mana = 20;
            Item.damage = 205;
            Item.knockBack = 3;
            Item.DamageType = DamageClass.Magic;


            Item.shoot = ModContent.ProjectileType<MADMineProj>();
            Item.useTime = Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.sentry = true;
            Item.noMelee = true;

            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
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
                AddIngredient<ProximityMines>(1).
                AddIngredient<AghanimShard>(1).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}