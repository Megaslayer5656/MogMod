using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class Yasha : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 66;
            Item.damage = 26; // compared to megashark with basic bullets
            Item.knockBack = 3;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.consumable = false;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.value = Item.buyPrice(0, 50, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
            Item.shootSpeed = 8f;
            Item.shoot = ModContent.ProjectileType<YashaProjectile>();
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 46;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<YashaProjectile>();
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("MythrilBar", 10).
                AddIngredient(ItemID.SoulofMight, 7).
                AddIngredient<FrigidCrystal>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}