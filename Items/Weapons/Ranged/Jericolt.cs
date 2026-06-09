using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class Jericolt : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public bool buffType = false;
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 30;

            Item.damage = 65;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = SoundID.Item41;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 12f;

            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-2, 2);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (buffType)
            {
                Projectile fireShot = Projectile.NewProjectileDirect(source, position + velocity.RotatedBy(-0.6 * player.direction) - velocity * 0.5f, velocity, type, damage, knockback, player.whoAmI);
                MogModGlobalProjectile mogProj = fireShot.MogMod();
                mogProj.fireBullet = true;
            }
            else
            {
                Projectile iceShot = Projectile.NewProjectileDirect(source, position + velocity.RotatedBy(-0.6 * player.direction) - velocity * 0.5f, velocity, type, damage, knockback, player.whoAmI);
                MogModGlobalProjectile mogProj = iceShot.MogMod();
                mogProj.iceBullet = true;
            }
            buffType = !buffType;
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PhoenixBlaster).
                AddIngredient(ItemID.Obsidian, 15).
                AddIngredient<FrigidCrystal>().
                AddIngredient(ItemID.IllegalGunParts).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}