using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class HeavensHalberd : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 80;
            Item.damage = 52;
            Item.knockBack = 5;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.consumable = false;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.value = Item.buyPrice(0, 50, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<HeavensHalberdProjectile>();
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 66;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<HeavensHalberdProjectile>();
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Yasha>(1).
                AddIngredient(ItemID.ScourgeoftheCorruptor, 1).
                AddIngredient(ItemID.SpectreBar, 10).
                AddIngredient(ItemID.SoulofFlight, 7).
                AddIngredient(ItemID.GiantHarpyFeather, 1).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}