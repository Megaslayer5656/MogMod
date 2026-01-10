using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Projectiles.MeleeProjectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class Daedalus : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        // these dont actually do anything
        Random random = new Random();

        public int attackDamage = 180;

        public int randUltraCrit;

        public bool ultraCrit = false;

        public bool hitsound = true;

        public static readonly SoundStyle daedalusSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/DaedalusCrit") //Shot sound effect
        {
            Volume = 1.4f,
            PitchVariance = .2f
        };
        public override void SetDefaults()
        {
            Item.width = 96;
            Item.height = 94;
            Item.DamageType = DamageClass.Melee;
            Item.damage = attackDamage;
            Item.knockBack = 8.5f;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.useTurn = false;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.rare = ItemRarityID.Red;

            // DAEDALUS CROSSBOW SLOP
            Item.shoot = ModContent.ProjectileType<DaedalusProjectile>();
            Item.shootSpeed = 10f;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 71;

        // Fires one large and two small projectiles which stay together in formation. USE FOR DAEDALUS CROSSBOW
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Play the Terra Blade sound upon firing
            SoundEngine.PlaySound(SoundID.Item60, position);

            // Center projectile
            int centerID = ModContent.ProjectileType<DaedalusProjectile>();
            int centerDamage = (damage / 2);
            Projectile.NewProjectile(source, position, velocity, centerID, centerDamage, knockback, player.whoAmI, 0f, 0f);

            // Side projectiles (these deal 75% damage)
            int sideID = ModContent.ProjectileType<DaedalusProj>();
            int sideDamage = ((int)(0.75f * centerDamage) / 2);
            Vector2 originalVelocity = velocity;
            velocity.Normalize();
            velocity *= 22f;
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 leftOffset = velocity.RotatedBy(MathHelper.PiOver4, default);
            Vector2 rightOffset = velocity.RotatedBy(-MathHelper.PiOver4, default);
            leftOffset -= 1.4f * velocity;
            rightOffset -= 1.4f * velocity;
            Projectile.NewProjectile(source, new Vector2(rrp.X + leftOffset.X, rrp.Y + leftOffset.Y), originalVelocity, sideID, sideDamage, knockback, player.whoAmI, 0f, 1f);
            Projectile.NewProjectile(source, new Vector2(rrp.X + rightOffset.X, rrp.Y + rightOffset.Y), originalVelocity, sideID, sideDamage, knockback, player.whoAmI, 0f, 2f);

            hitsound = true;
            return false;
        }

        // On-hit, explode for extra damage.
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Dazed, 480);
            target.AddBuff(BuffID.Daybreak, 480);
            OnHitEffects(player, target.Center);
        }
        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Dazed, 480);
            target.AddBuff(BuffID.Daybreak, 480);
            OnHitEffects(player, target.Center);
        }

        private void OnHitEffects(Player player, Vector2 targetPos)
        {
            if (hitsound)
            {
                SoundEngine.PlaySound(daedalusSound with { Volume = 0.5f, Pitch = 0.2f, PitchVariance = 0.2f, MaxInstances = -1}, player.Center);
                //hitsound = false;
            }
            int trueMeleeID = ModContent.ProjectileType<DaedalusBoom>();
            int trueMeleeDamage = (int)player.GetTotalDamage<MeleeDamageClass>().ApplyTo(0.85f * Item.damage);
            var source = player.GetSource_ItemUse(Item);
            Projectile.NewProjectile(source, targetPos, Vector2.Zero, trueMeleeID, trueMeleeDamage, Item.knockBack, player.whoAmI, 0.0f, 0.0f);
        }

        // Spawn some fancy dust while swinging
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            int dustCount = Main.rand.Next(3, 6);
            Vector2 corner = new Vector2(hitbox.X + hitbox.Width / 4, hitbox.Y + hitbox.Height / 4);
            for (int i = 0; i < dustCount; ++i)
            {
                // Pick a random dust to spawn
                int dustID;
                switch (Main.rand.Next(5))
                {
                    case 0:
                    case 1:
                        dustID = 105;
                        break;
                    case 2:
                        dustID = 183;
                        break;
                    default:
                        dustID = 296;
                        break;
                }
                int idx = Dust.NewDust(corner, hitbox.Width / 2, hitbox.Height / 2, dustID);
                Main.dust[idx].noGravity = true;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Crystalys>(1).
                AddIngredient(ItemID.PiercingStarlight, 1).
                AddIngredient(ItemID.FragmentSolar, 8).
                AddIngredient<UltimateOrb>(3).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
