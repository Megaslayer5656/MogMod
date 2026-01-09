using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class ArchbeastParagon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public const int MaxCharge = 5;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 96;
            Item.damage = 235;
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
        // TODO: give a right click
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 46;
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }

        // Spawning the holdout cannot consume ammo
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] > 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // for some reason it wont let you channel right click, so i gotta fix that
            if (player.altFunctionUse == 2)
            {
                Vector2 shootVelocity = velocity;
                Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);

                Projectile.NewProjectile(source, position, shootDirection, ModContent.ProjectileType<ArchbeastHoldout>(), damage, knockback, player.whoAmI);
                // stop player from moving while shooting
                player.AddBuff(ModContent.BuffType<DragonPiercerShot>(), 200);
                return false;
            }
            else
            {
                Vector2 shootVelocity = velocity;
                Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
                // Charge-up. Done via a holdout projectile.
                Projectile.NewProjectile(source, position, shootDirection, ModContent.ProjectileType<ArchbeastBowCharge>(), damage, knockback, player.whoAmI);
                return false;
            }
        }
    }
}