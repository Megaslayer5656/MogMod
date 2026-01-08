using MogMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class ArchbeastHoldout : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override string Texture => "MogMod/Items/Weapons/Ranged/ArchbeastParagon";
        public ref float HoldTimer => ref Projectile.ai[0];
        public ref float ShootTimer => ref Projectile.ai[1];
        public int ShootCount = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;

            // Adjust the drawing to change how it appears when held
            DrawOffsetX = -3;
            DrawOriginOffsetY = -17;
        }
        public override bool? CanDamage() => false;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);

            // HoldTimer counts how long the weapon has been used. It helps control how fast the weapon animates and shoots arrows.
            float numb = 1f;
            HoldTimer += numb;
            int initialShootDelay = 195;

            ShootTimer += 1f;
            bool shouldShootArrow = false;
            if (ShootTimer >= initialShootDelay)
            {
                ShootTimer = 0f;
                shouldShootArrow = true;
            }

            // Sound and dust are separate from the code shooting arrows because they need to run on other clients as well.
            if (Projectile.soundDelay <= 0)
            {
                Projectile.soundDelay = initialShootDelay;
                // Prevents a shoot sound from playing when the projectile initially spawns
                if (HoldTimer != numb)
                {
                    SoundEngine.PlaySound(SoundID.Item5, player.position);
                }
            }

            Vector2 dustSpawnLocation = Projectile.Center + new Vector2(15, 0).RotatedBy(Projectile.rotation - (Projectile.direction == 1 ? 0 : MathHelper.Pi)) - new Vector2(8, 8);
            for (int i = 0; i < 2; i++)
            {
                var dust = Dust.NewDustDirect(dustSpawnLocation, 16, 16, DustID.GoldFlame, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f, 100);
                dust.velocity *= 0.66f;
                dust.noGravity = true;
                dust.scale = 0.6f;
            }

            if (shouldShootArrow && Main.myPlayer == Projectile.owner)
            {
                Item heldItem = player.HeldItem;
                if (player.HasAmmo(heldItem) && !player.noItems && !player.CCed)
                {
                    float holdoutDistance = ArchbeastParagon.HoldoutDistance * Projectile.scale;
                    Vector2 holdoutOffset = holdoutDistance * Vector2.Normalize(Main.MouseWorld - playerCenter);
                    if (holdoutOffset.X != Projectile.velocity.X || holdoutOffset.Y != Projectile.velocity.Y)
                    {
                        Projectile.netUpdate = true;
                    }

                    // Set the projectile velocity, which is actually the holdout offset for held projectiles.
                    Projectile.velocity = holdoutOffset;

                    int projectileCount = 1;

                    for (int j = 0; j < projectileCount; j++)
                    {
                        // Calculate a spawn location, taking into account the muzzle placement and a random variation
                        var spawnLocation = playerCenter + holdoutOffset + Main.rand.NextVector2Circular(6, 6);
                        bool ammoConsumed = player.PickAmmo(heldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId);

                        if (ammoConsumed)
                        {
                            var source = player.GetSource_ItemUse_WithPotentialAmmo(heldItem, usedAmmoItemId);
                            // change to custom inf pierce proj
                            Projectile.NewProjectile(source, spawnLocation, Vector2.Normalize(Projectile.velocity) * (speed * 2), ModContent.ProjectileType<ArchbeastArrow>(), damage * 8, knockBack * 3, Projectile.owner);
                        }
                    }
                    Projectile.Kill();
                }
            }

            Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
            Projectile.spriteDirection = Projectile.direction;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
            Projectile.Center = playerCenter;
            float rotationOffset = Projectile.spriteDirection == -1 ? MathHelper.Pi : 0;
            Projectile.rotation = Projectile.velocity.ToRotation() + rotationOffset;
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            Projectile.timeLeft = 2;
        }
    }
}