using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class SinisterSpreader : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 68;
            Item.height = 26;

            Item.damage = 48;
            Item.knockBack = 4f;
            Item.ArmorPenetration = 25;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;

            Item.useTime = 14;
            Item.useAnimation = 42;
            Item.reuseDelay = 14;
            Item.useLimitPerAnimation = 3;

            Item.useAmmo = AmmoID.Gel;
            Item.consumeAmmoOnFirstShotOnly = true;
            Item.shootSpeed = 12f;
            Item.shoot = ProjectileID.PurificationPowder;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item34;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int bulletAmt = 6;
            int spread = 40;
            for (int index = 0; index < bulletAmt; ++index)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-spread, spread + 1) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-spread, spread + 1) * 0.05f;
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<DragonFlayerProj>(), damage, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-4, 0);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SpookyEssence>(20).
                AddIngredient(ItemID.SpookyWood, 150).
                AddRecipeGroup("AnyTorch").
                AddTile(TileID.Anvils).
                Register();
        }
    }
}