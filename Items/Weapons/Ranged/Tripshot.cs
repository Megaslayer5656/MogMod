using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class Tripshot : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public int shotFired = 0;
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 26;

            Item.damage = 22;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.useTime = 4;
            Item.useAnimation = 12;
            Item.reuseDelay = Item.useAnimation + 4;
            Item.useLimitPerAnimation = 3;
            Item.consumeAmmoOnFirstShotOnly = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = SoundID.Item41;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 10f;

            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float Spread = 0.1f;
            SoundEngine.PlaySound(SoundID.Item41, player.Center);
            switch (shotFired)
            {
                case 0:
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(Spread), type, damage, knockback, player.whoAmI);
                    shotFired++;
                    break;
                case 1:
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                    shotFired++;
                    break;
                case 2:
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(-Spread), type, damage, knockback, player.whoAmI);
                    shotFired = 0;
                    break;
            }
            return false;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-2f, 0f);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Boomstick).
                AddIngredient<FuciumBar>(8).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}