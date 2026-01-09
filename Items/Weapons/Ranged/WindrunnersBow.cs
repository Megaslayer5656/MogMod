using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Projectiles.RangedProjectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    // ExampleHeldProjectileWeapon is a "held projectile" weapon. This means the weapon is actually a projectile. Since it is a projectile, it can have custom animation or run custom logic. Doing custom animation or logic with a regular item is sometimes difficult, which is why more advanced weapons sometimes use held projectiles. Some other "held projectile" items in ExampleMod include ExampleDrill, ExampleLastPrism, and ExampleCustomSwingSword.
    // This example is technically a bow weapon and shoots arrows, but if you are looking for a simple bow example, just use ExampleGun as a guide and change the useAmmo, there is nothing more to making a basic bow weapon.
    // This example demonstrates the weapon animation and fire rate acceleration features of the Laser Machinegun and the occasional secondary projectile feature of Vortex Beater or Phantom Phoenix.
    // This example also teaches manually picking and consuming ammo using Player.PickAmmo.
    public class WindrunnersBow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public static readonly SoundStyle windrunnerShoot = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/WindrunnerShoot")
        {
            Volume = 1.1f,
            PitchVariance = .2f,
            MaxInstances = 2
        };
        public const int HoldoutDistance = 20;
        public override bool AltFunctionUse(Player player) => true;
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = Item.useTime = 20;
            // By convention, the shootSpeed of a held projectile weapon usually corresponds to how far out the projectile is held. This will unfortunately also affect the velocity of the ammo projectiles this weapon spawns, so we won't be using shootSpeed as the holdout distance in this example.
            Item.shootSpeed = 6f;
            Item.knockBack = 2f;
            Item.width = 56;
            Item.height = 26;
            Item.damage = 9;
            Item.shoot = ModContent.ProjectileType<WindrunnerHoldout>();
            Item.useAmmo = AmmoID.Arrow;
            Item.rare = ItemRarityID.Green;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = false;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 21;
        // so it doesnt look weird
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0f, 0f); // 0,0 is player center
        }
        // determines the properties for each firing mode
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = null;
                Item.noUseGraphic = true;
                Item.autoReuse = false;
                Item.useTime = Item.useAnimation = 20;
            }
            else
            {
                Item.UseSound = SoundID.Item5;
                Item.noUseGraphic = false;
                Item.autoReuse = true;
                Item.useTime = Item.useAnimation = 9;
            }
            return true;
        }
        // fires powershot if right clicked
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            if (player.altFunctionUse == 2)
            {
                SoundEngine.PlaySound(windrunnerShoot); // starts playing at current position, doesnt update the sound with where you are
                // Since this item will attempt to shoot an ammo item, we need to set it back to the actual held projectile here.
                type = ModContent.ProjectileType<WindrunnerHoldout>();
                // The velocity value provided is not correct, so we need to calculate a new velocity since velocity for held projectiles is actually the holdout offset.
                velocity = Vector2.Normalize(velocity) * HoldoutDistance;

                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer);
                // stop player from moving while shooting
                player.AddBuff(ModContent.BuffType<ChargingShot>(), 70);
                return false;
            }
            else
            {
                return true;
            }
        }
        // gives variation in shots fired like minislark or switch
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                Vector2 muzzleOffset = Vector2.Normalize(velocity) * 5f;
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(3));
                if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                {
                    position += muzzleOffset;
                }
            }
        }
        // prevents the item from consuming ammo when initially used.
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (player.altFunctionUse == 2)
            {
                    if (player.ItemTimeIsZero)
                    {
                        return false;
                    }
            }
            return true;
        }
    }
}