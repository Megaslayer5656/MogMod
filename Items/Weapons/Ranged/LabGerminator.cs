using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class LabGerminator : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 83;
            Item.height = 52;

            Item.damage = 13;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;

            Item.useTime = 10;
            Item.useAnimation = 30;
            Item.reuseDelay = 20;
            Item.useLimitPerAnimation = 3;

            //Item.useAmmo = AmmoID.Bullet;
            //Item.consumeAmmoOnFirstShotOnly = true;
            Item.shootSpeed = 12f;
            Item.shoot = ProjectileID.PurificationPowder;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item5;

            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-8, 0);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.Item5);
            for (int i = 0; i < 2; i++)
            {
                Vector2 splinterVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 * 0.3);
                Projectile.NewProjectile(source, position, splinterVelocity, ModContent.ProjectileType<LabGerminatorProj>(), damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}