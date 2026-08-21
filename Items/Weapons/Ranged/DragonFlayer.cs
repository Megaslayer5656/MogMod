using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class DragonFlayer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public static int AmmoSavedPercent = 50;
        public static int ArmorPenetration = 50;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AmmoSavedPercent, ArmorPenetration);
        public override void SetDefaults()
        {
            Item.width = 118;
            Item.height = 64;

            Item.damage = 75;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;

            Item.useTime = 3;
            Item.reuseDelay = 3;
            Item.useAnimation = 12;
            Item.useLimitPerAnimation = 4;

            Item.useAmmo = AmmoID.Gel;
            Item.consumeAmmoOnFirstShotOnly = true;
            Item.shootSpeed = 22f;
            Item.shoot = ModContent.ProjectileType<DragonFlayerProj>();

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item34;

            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //for (int i = 0; i < 2; i++)
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.12f), type, damage, knockback, player.whoAmI);
            return false;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-46, 0);
        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) >= AmmoSavedPercent;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LunarBar, 12).
                AddIngredient<SoulFragment>(5).
                AddIngredient<ScorchedCore>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
