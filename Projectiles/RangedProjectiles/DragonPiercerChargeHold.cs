using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Weapons.Ranged;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class DragonPiercerChargeHold : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override string Texture => "MogMod/Items/Weapons/Ranged/DragonPiercer";
        public static readonly SoundStyle weakCharge = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/bowChargeWeak")
        {
            Volume = 1.1f,
            PitchVariance = .2f,
            MaxInstances = 5
        };
        public static readonly SoundStyle strongCharge = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/bowChargeStrong")
        {
            Volume = 1.1f,
            PitchVariance = .2f,
            MaxInstances = 2
        };
        private Player Owner => Main.player[Projectile.owner];
        public ref float HoldTimer => ref Projectile.ai[0];
        public ref float ShootTimer => ref Projectile.ai[1];
        public int ShootCount = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 96;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.5f;
            Vector2 tipAdjustment = (Projectile.velocity * Projectile.width * 0.25f);
            float numb = 1f;
            HoldTimer += numb;
            int initialShootDelay = 120;
            ShootTimer += 1f;
            bool shouldShootArrow = false;
            if (ShootTimer >= initialShootDelay)
            {
                ShootTimer = 0f;
                SoundEngine.PlaySound(strongCharge);
                for (int i = 0; i < 75; i++)
                {
                    float colorRando = Main.rand.NextFloat(0, 1);
                    float offsetAngle = MathHelper.TwoPi * i / 75f;
                    float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 3D);
                    float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 3D);
                    Vector2 puffDustVelocity = new Vector2(unitOffsetX, unitOffsetY) * 2.5f;
                    Dust charged = Dust.NewDustPerfect(tipPosition + tipAdjustment, 267, puffDustVelocity);
                    charged.scale = 0.75f;
                    charged.fadeIn = 0.5f;
                    charged.color = Color.Lerp(Color.Yellow, Color.Gold, colorRando);
                    charged.noGravity = true;
                }
                shouldShootArrow = true;
            }
            if (ShootTimer == 30f || ShootTimer == 60f || ShootTimer == 90f)
            {
                SoundEngine.PlaySound(weakCharge);
                for (int i = 0; i < 75; i++)
                {
                    float colorRando = Main.rand.NextFloat(0, 1);
                    float offsetAngle = MathHelper.TwoPi * i / 75f;
                    float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 3D);
                    float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 3D);
                    Vector2 puffDustVelocity = new Vector2(unitOffsetX, unitOffsetY) * 2f;
                    Dust charged = Dust.NewDustPerfect(tipPosition + tipAdjustment, 267, puffDustVelocity);
                    charged.scale = 0.5f;
                    charged.fadeIn = 0.5f;
                    charged.color = Color.Lerp(Color.White, Color.GhostWhite, colorRando);
                    charged.noGravity = true;
                }
            }
            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * -12f;
            if (ShootTimer >= 30)
            {
                for (int i = 0; i <= 5; i++)
                {
                    Dust dust = Dust.NewDustPerfect(tipPosition + Projectile.velocity * 15, DustID.RainbowTorch, shootVelocity.RotatedByRandom(MathHelper.ToRadians(6f)) * Main.rand.NextFloat(0.2f, 1.2f), 0, Color.Goldenrod, Main.rand.NextFloat(1f, 2.3f));
                    dust.scale = 0.6f;
                    dust.alpha = 100;
                    dust.noGravity = true;
                }
                if (ShootTimer >= 60)
                {
                    for (int i = 0; i <= 5; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(tipPosition + Projectile.velocity * 15, DustID.Torch, shootVelocity.RotatedByRandom(MathHelper.ToRadians(9f)) * Main.rand.NextFloat(0.2f, 1.2f), 0, default, Main.rand.NextFloat(1f, 2.3f));
                        dust.scale = 0.8f;
                        dust.alpha = 100;
                        dust.noGravity = true;
                    }
                    if (ShootTimer >= 90)
                    {
                        for (int i = 0; i <= 5; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(tipPosition + Projectile.velocity * 15, DustID.Flare, shootVelocity.RotatedByRandom(MathHelper.ToRadians(12f)) * Main.rand.NextFloat(0.2f, 1.2f), 0, default, Main.rand.NextFloat(1f, 2.3f));
                            dust.scale = 1f;
                            dust.alpha = 100;
                            dust.noGravity = true;
                        }
                    }
                }
            }
            if (shouldShootArrow && Main.myPlayer == Projectile.owner)
            {
                Item heldItem = player.HeldItem;
                if (player.HasAmmo(heldItem) && !player.noItems && !player.CCed)
                {
                    float holdoutDistance = WindrunnersBow.HoldoutDistance * Projectile.scale;
                    Vector2 holdoutOffset = holdoutDistance * Vector2.Normalize(Main.MouseWorld - tipPosition);
                    if (holdoutOffset.X != Projectile.velocity.X || holdoutOffset.Y != Projectile.velocity.Y)
                    {
                        Projectile.netUpdate = true;
                    }
                    Projectile.velocity = holdoutOffset;
                    int projectileCount = 1;
                    for (int j = 0; j < projectileCount; j++)
                    {
                        var spawnLocation = tipPosition + holdoutOffset + Main.rand.NextVector2Circular(6, 6);
                        bool ammoConsumed = player.PickAmmo(heldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId);
                        if (ammoConsumed)
                        {
                            var source = player.GetSource_ItemUse_WithPotentialAmmo(heldItem, usedAmmoItemId);
                            Projectile.NewProjectile(source, spawnLocation, Vector2.Normalize(Projectile.velocity) * (speed), ModContent.ProjectileType<ArchbeastArrow>(), damage * 10, knockBack * 3, Projectile.owner);
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
            Projectile.Center = tipPosition;
            float rotationOffset = Projectile.spriteDirection == -1 ? MathHelper.Pi : 0;
            Projectile.rotation = Projectile.velocity.ToRotation() + rotationOffset;
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            Projectile.timeLeft = 2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float BoltAngle = 0f;
            float Shift = 0f;
            Shift = 1 - (ShootTimer / HoldTimer);
            Color Transparency = Projectile.GetAlpha(lightColor) * (1 - Shift);
            var BoltTexture = ModContent.Request<Texture2D>("MogMod/Projectiles/RangedProjectiles/ArchbeastArrow").Value;
            Vector2 PointingTo = new Vector2((float)Math.Cos(Projectile.rotation + BoltAngle), (float)Math.Sin(Projectile.rotation + BoltAngle));
            Vector2 ShiftDown = PointingTo.RotatedBy(-MathHelper.PiOver2);
            float FlipFactor = Owner.direction < 0 ? MathHelper.Pi : 0f;
            Vector2 drawPosition = Owner.Center + PointingTo.RotatedBy(FlipFactor) * (10f + (Shift * 40)) - ShiftDown.RotatedBy(FlipFactor) * (BoltTexture.Width / 2) - Main.screenPosition;
            Main.EntitySpriteDraw(BoltTexture, drawPosition, null, Transparency, Projectile.rotation + (BoltAngle * 1f) + MathHelper.PiOver2 + FlipFactor, BoltTexture.Size(), 1f, 0, 0);
            return true;
        }
    }
}