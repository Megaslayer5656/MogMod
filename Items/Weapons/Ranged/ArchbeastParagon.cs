using MogMod.Projectiles.RangedProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class ArchbeastParagon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public const int MaxCharge = 5;
        public const int HoldoutDistance = 20;
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 62;
            Item.damage = 70;
            Item.knockBack = 3f;
            Item.shootSpeed = 15f;
            Item.useTime = Item.useAnimation = 30;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item20;
            Item.shoot = ModContent.ProjectileType<ArchbeastBowCharge>();
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        // Spawning the holdout cannot consume ammo
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] > 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 shootVelocity = velocity;
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            // Charge-up. Done via a holdout projectile.
            Projectile.NewProjectile(source, position, shootDirection, ModContent.ProjectileType<ArchbeastBowCharge>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}