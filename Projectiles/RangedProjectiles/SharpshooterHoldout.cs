using Microsoft.Xna.Framework;
using MogMod.Items.Weapons.Ranged;
using MogMod.Utilities;
using ReLogic.Utilities;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class SharpshooterHoldout : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override string Texture => "MogMod/Items/Weapons/Ranged/Sharpshooter";
        private Player Owner => Main.player[Projectile.owner];
        private ref float CurrentChargingFrames => ref Projectile.ai[0];
        private ref float CurrentCharge => ref Projectile.ai[1];
        public const float MinCharge = 0f;
        private readonly float[] amount = [0f, 20f, 40f, 60f, 80f];
        public static bool FullyCharged = false;
        public SlotId AudSlot;
        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            CurrentCharge = -50f;
            FullyCharged = false;
        }
        public override void AI()
        {
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.5f;
            Vector2 tipAdjustment = (Projectile.velocity * Projectile.width * 0.25f);
            if (Owner.CantUseHoldout())
            {
                if (CurrentCharge < MinCharge)
                {
                    Projectile.Kill();
                    return;
                }
                else ShootProjectile(tipPosition);
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();
            }
            else
            {
                if (Owner.HasAmmo(Owner.ActiveItem()))
                {
                    if (CurrentCharge <= Sharpshooter.MaxCharge)
                    {
                        if (CurrentCharge == Sharpshooter.MaxCharge)
                        {
                            FullyCharged = true;
                            SoundEngine.PlaySound(SoundID.Item23);
                            ShootProjectile(tipPosition);
                        }
                        ++CurrentCharge;
                        Item heldItem = Owner.ActiveItem();
                        if (CurrentCharge == MinCharge)
                        {
                            Owner.PickAmmo(heldItem, out _, out float shootSpeed, out int damage, out float knockback, out _);
                            Projectile.damage = damage;
                            Projectile.knockBack = knockback;
                            SoundEngine.PlaySound(SoundID.Item108);
                        }
                    }
                }
                if (CurrentCharge >= MinCharge)
                {
                    if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                    {
                        ChargeSound.Position = Projectile.Center;
                        ChargeSound.Pitch = Utils.Remap(CurrentCharge, 0, Sharpshooter.MaxCharge, -0.4f, 0f);
                        ChargeSound.Volume = Utils.Remap(CurrentCharge, 0, Sharpshooter.MaxCharge, 0.2f, 1f) * 100;
                    }
                    else AudSlot = SoundEngine.PlaySound(SoundID.DD2_KoboldIgniteLoop with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
                    int dustSpot = 6;
                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * -12f;
                    for (int i = 0; i <= 5; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(tipPosition - Projectile.velocity * dustSpot, DustID.Flare, shootVelocity.RotatedByRandom(MathHelper.ToRadians(12f)) * Main.rand.NextFloat(0.2f, 1.2f), 0, default, Main.rand.NextFloat(1f, 2.3f));
                        dust.scale = 1f;
                        dust.alpha = 100;
                        dust.noGravity = true;
                    }
                    if (amount.Contains(CurrentCharge)) SoundEngine.PlaySound(SoundID.Item17);
                }
                if (CurrentCharge < MinCharge) if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();
            }
            UpdateProjectileHeldVariables(armPosition);
            ManipulatePlayerVariables();
        }
        public void ShootProjectile(Vector2 tipPosition)
        {
            if (Main.myPlayer != Projectile.owner)
                return;
            SoundEngine.PlaySound(SoundID.Item98);
            Vector2 shootVelocity = Projectile.velocity * 20f;
            Owner.velocity += shootVelocity.SafeNormalize(Vector2.UnitX) * -10f;
            Projectile.damage = (int)(Projectile.damage * ((CurrentCharge * 0.05f) + 1));
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, shootVelocity, ModContent.ProjectileType<SharpshooterProj>(), Projectile.damage, Projectile.knockBack * ((CurrentCharge * .01f) + 1f), Projectile.owner);
            CurrentCharge = 0;
            Projectile.Kill();
            if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();
        }
        private void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float interpolant = Utils.GetLerpValue(5f, 25f, Owner.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Owner.SafeDirectionTo(Main.MouseWorld), interpolant);
                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }
            Projectile.position = armPosition - Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.Zero) * 25f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            int oldDirection = Projectile.spriteDirection;
            if (oldDirection == -1)
                Projectile.rotation += MathHelper.Pi;
            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
            if (Projectile.spriteDirection != oldDirection)
                Projectile.rotation -= MathHelper.Pi;
            Projectile.timeLeft = 3;
        }
        private void ManipulatePlayerVariables()
        {
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }
        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound))
                ChargeSound?.Stop();
        }
        public override bool? CanDamage() => false;
    }
}