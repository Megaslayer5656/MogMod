using Microsoft.Xna.Framework;
using MogMod.Common.Config;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Weapons.Ranged;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class AXMCHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<AXMC>();
        public static readonly SoundStyle UseSound = new($"{nameof(MogMod)}/Sounds/SE/AXMCShot") { Volume = 2.25f, PitchVariance = .02f };
        public override float MaxOffsetLengthFromArm => 53f;
        public override float OffsetXUpwards => 0f;
        public override float BaseOffsetY => -8f;
        public override float OffsetYDownwards => 16f;
        public ref float ShootTimer => ref Projectile.ai[0];
        public ref float ReloadTimer => ref Projectile.ai[1];
        public ref float LastUseTime => ref Projectile.ai[2];
        public int Cap = 10;
        public int shootTime = AXMC.reloadTime;
        public int attackTime = 0;
        public int maxShots = AXMC.maxShots;
        public override Vector2 GunTipPosition => Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.5f;
        public override void KillHoldoutLogic() { }
        public override void SendExtraAIHoldout(BinaryWriter writer)
        {
            writer.Write(Projectile.spriteDirection);
        }
        public override void ReceiveExtraAIHoldout(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadInt32();
        }
        public override void HoldoutAI()
        {
            MogPlayer mogPlayer = Owner.MogMod();
            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 20;
            var attackSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
            if (attackSpeed > Cap) attackSpeed = Cap;
            if (attackSpeed != 0f) attackSpeed = 1f / attackSpeed;
            attackTime = (int)(shootTime * attackSpeed);

            SetUsage = false;
            bool doingNothing = ReloadTimer == 0;
            if (LastUseTime == 0 || doingNothing) LastUseTime = Owner.HeldItem.useAnimation;
            if (!doingNothing) Owner.itemTime = Owner.itemAnimation = 5;

            if ((Owner.HeldItem.type != ModContent.ItemType<AXMC>() && doingNothing) || (doingNothing && (Main.mapFullscreen || Owner.mouseInterface)) || Owner.dead)
            {
                Projectile.Kill();
                return;
            }

            bool hasAmmo = Owner.PickAmmo(HeldItem, out _, out _, out _, out _, out _, true);
            bool leftShootChecks = Owner.whoAmI == Main.myPlayer && (Main.mouseLeft && !Main.mapFullscreen && !Owner.mouseInterface && ShootTimer <= 0 && ReloadTimer <= 0) && hasAmmo;

            // if we ran out of ammo, reload
            if (mogPlayer.axmcShots != maxShots && KeybindSystem.FirstWeaponKeybind.JustPressed && hasAmmo || ReloadTimer != 0)
            {
                ReloadTimer++;
                if (ReloadTimer == 2 || ReloadTimer == attackTime / 2)
                {
                    if (ReloadTimer == 2)
                    {

                        if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                        {
                            string goreType = "AXMCMag";
                            Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, shootVelocity.RotatedBy(2f * -Owner.direction) * Main.rand.NextFloat(0.6f, 0.7f), Mod.Find<ModGore>(goreType).Type);
                        }
                        Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _);
                    }
                    SoundEngine.PlaySound(SoundID.Item149 with { Pitch = -0.2f }, Owner.Center);
                    SoundEngine.PlaySound(SoundID.Item108 with { Pitch = -0.3f }, Owner.Center);
                    if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= ReloadTimer == 2 ? 4f : 2f;
                }
                if (ReloadTimer >= attackTime)
                {
                    SoundEngine.PlaySound(SoundID.ResearchComplete with { Volume = 0.35f, Pitch = -0.3f }, Owner.Center);

                    int totalDusts = 5;
                    float starAngle = MathHelper.Pi / totalDusts;
                    for (int i = 0; i < totalDusts; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB);
                        Vector2 vel = (MathHelper.TwoPi * i / totalDusts).ToRotationVector2().RotatedBy(starAngle) * 2f;
                        Dust dust2 = Dust.NewDustPerfect(GunTipPosition, DustID.FireworksRGB, vel, 80, Color.SandyBrown, 1.2f);
                        dust2.noGravity = true;
                    }

                    mogPlayer.axmcShots = maxShots;
                    ReloadTimer = 0;
                }
            }
            else
            {
                if (leftShootChecks) Shoot(shootVelocity);
            }
            if (ShootTimer > 0) ShootTimer--;
        }
        public void Shoot(Vector2 shootVelocity)
        {
            MogPlayer mogPlayer = Owner.MogMod();
            if (mogPlayer.axmcShots <= 0)
            {
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    SoundEngine.PlaySound(SoundID.Item17 with { PitchVariance = 0.2f }, Owner.Center);
                    if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 2f;
                }
                return;
            }

            SoundEngine.PlaySound(UseSound, Owner.Center);
            Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, Vector2.Zero, 100, Color.BlanchedAlmond, Main.rand.NextFloat(0.8f, 1.2f));
            for (int i = 0; i <= 12; i++)
            {
                Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, (shootVelocity * Main.rand.NextFloat(0.2f, 1.1f)).RotatedByRandom(0.4f), 0, default);
                dust2.noGravity = true;
                dust2.scale = Main.rand.NextFloat(0.8f, 1.4f);
            }
            if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 30f; // visual recoil effect
            Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _, true);
            if (Main.myPlayer == Projectile.owner)
            {
                // reduce ammo by 1
                if (mogPlayer.axmcShots > 0) mogPlayer.axmcShots--;
                var source = Projectile.GetSource_FromThis();
                int type = ammo;
                if (ammo == ProjectileID.Bullet)
                {
                    type = ModContent.ProjectileType<APLapuaProj>();
                    bulletDamage = (int)(bulletDamage * 2f);
                    knockback *= 2f;
                }
                Owner.velocity += shootVelocity.SafeNormalize(Vector2.UnitX) * (Main.zenithWorld ? -100f : -18f);
                Vector2 shootPos = Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.25f;
                Projectile.NewProjectile(source, shootPos, shootVelocity, type, bulletDamage, knockback, Projectile.owner);
                if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                {
                    string goreType = "AXMCCasing";
                    Vector2 spawnOffset = new(0, -11f);
                    Vector2 spawnPosition = Projectile.Center + (-Projectile.velocity * 5) + spawnOffset;
                    Gore.NewGore(Projectile.GetSource_FromAI(), spawnPosition, -shootVelocity * 4f, Mod.Find<ModGore>(goreType).Type);
                }
                ShootTimer = attackTime / 3;
            }
        }
    }
}