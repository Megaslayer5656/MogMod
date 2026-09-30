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
    public class CrescentMoonHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<CrescentMoon>();
        public override string Texture => "MogMod/Projectiles/RangedProjectiles/CrescentMoonHoldout";
        public override float MaxOffsetLengthFromArm => 24f;
        public override float BaseOffsetY => -3f;
        public override float OffsetYDownwards => 4f;
        public ref float ShootTimer => ref Projectile.ai[0];
        public ref float ReloadTimer => ref Projectile.ai[1];
        public int LastUseTime = 0;
        public int CooldownTimer = 0;
        public int Cap = 10;
        public int shootTime = CrescentMoon.reloadTime;
        public int attackTime = 0;
        public int maxShots = CrescentMoon.maxShots;
        public override Vector2 GunTipPosition => Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.5f;
        public override void KillHoldoutLogic() { }
        public override void SendExtraAIHoldout(BinaryWriter writer)
        {
            writer.Write(LastUseTime);
            writer.Write(CooldownTimer);
            writer.Write(Projectile.spriteDirection);
        }
        public override void ReceiveExtraAIHoldout(BinaryReader reader)
        {
            LastUseTime = reader.ReadInt32();
            CooldownTimer = reader.ReadInt32();
            Projectile.spriteDirection = reader.ReadInt32();
        }
        public override void HoldoutAI()
        {
            ModifyArmPosition = false;
            if (Projectile.ai[2] == 1) ArmPosition = Owner.RotatedRelativePoint(Owner.MountedCenter + new Vector2(-5, 3), true);
            else ArmPosition = Owner.RotatedRelativePoint(Owner.MountedCenter + new Vector2(5, -3), true);

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

            if ((Owner.HeldItem.type != ModContent.ItemType<CrescentMoon>() && doingNothing) || (doingNothing && (Main.mapFullscreen || Owner.mouseInterface)) || Owner.dead)
            {
                Projectile.Kill();
                return;
            }

            bool hasAmmo = Owner.PickAmmo(HeldItem, out _, out _, out _, out _, out _, true);
            bool leftShootChecks = Owner.whoAmI == Main.myPlayer && (Main.mouseLeft && Main.mouseLeftRelease && !Main.mapFullscreen && !Owner.mouseInterface && ShootTimer <= 0 && ReloadTimer <= 0 && Projectile.ai[2] == 1f) && hasAmmo;
            bool rightShootChecks = Owner.whoAmI == Main.myPlayer && (Owner.MogMod().mouseRight && Main.mouseRightRelease && !Main.mapFullscreen && !Owner.mouseInterface && ShootTimer <= 0 && ReloadTimer <= 0 && Projectile.ai[2] == 0f) && hasAmmo;

            bool noLeftAmmo = mogPlayer.leftCrescentMoonShots < maxShots && Projectile.ai[2] == 0f;
            bool noRightAmmo = mogPlayer.rightCrescentMoonShots < maxShots && Projectile.ai[2] == 1f;

            // if we ran out of ammo, reload
            if (((noLeftAmmo || noRightAmmo) && KeybindSystem.FirstWeaponKeybind.Current && hasAmmo && CooldownTimer <= 0f) || ReloadTimer != 0)
            {
                ShootTimer = attackTime / maxShots;
                ReloadTimer++;
                if (ReloadTimer <= (attackTime / maxShots))
                {
                    if (ReloadTimer == 2)
                    {
                        /*
                        if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                        {
                            string goreType = "RigGunMag";
                            Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, shootVelocity.RotatedBy(2f * -Owner.direction) * Main.rand.NextFloat(0.6f, 0.7f), Mod.Find<ModGore>(goreType).Type);
                        }
                        */
                        if (noLeftAmmo) mogPlayer.leftCrescentMoonShots++;
                        if (noRightAmmo) mogPlayer.rightCrescentMoonShots++;
                        Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _);
                        SoundEngine.PlaySound(SoundID.Item149 with { Pitch = -0.2f }, Owner.Center);
                        SoundEngine.PlaySound(SoundID.Item108 with { Pitch = -0.3f }, Owner.Center);
                        if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 4f;
                    }
                    if (ReloadTimer >= (attackTime / maxShots))
                    {
                        if ((Projectile.ai[2] == 1f && mogPlayer.leftCrescentMoonShots >= maxShots) || (Projectile.ai[2] == 0f && mogPlayer.rightCrescentMoonShots >= maxShots))
                        {
                            SoundEngine.PlaySound(SoundID.ResearchComplete with { Volume = 0.35f, Pitch = -0.3f }, Owner.Center);

                            int totalDusts = 5;
                            float starAngle = MathHelper.Pi / totalDusts;
                            for (int i = 0; i < totalDusts; i++)
                            {
                                Dust chargefull = Dust.NewDustPerfect(GunTipPosition, DustID.FireworksRGB);
                                Vector2 vel = (MathHelper.TwoPi * i / totalDusts).ToRotationVector2().RotatedBy(starAngle) * 2f;
                                Dust dust2 = Dust.NewDustPerfect(GunTipPosition, DustID.FireworksRGB, vel, 80, Projectile.ai[2] == 1f ? Color.Turquoise : Color.SeaGreen, 1.2f);
                                dust2.noGravity = true;
                            }
                        }
                        ReloadTimer = 0;
                    }
                }
            }
            else
            {
                if (leftShootChecks) Shoot(shootVelocity, Projectile.ai[2]);
                else if (rightShootChecks) Shoot(shootVelocity, Projectile.ai[2]);
            }
            if (ShootTimer > 0) ShootTimer--;
            if (CooldownTimer > 0) CooldownTimer--;
        }
        public void Shoot(Vector2 shootVelocity, float leftClick)
        {
            bool left = leftClick == 0f;
            MogPlayer mogPlayer = Owner.MogMod();
            if (left && mogPlayer.leftCrescentMoonShots <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item17 with { PitchVariance = 0.2f }, Owner.Center);
                if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 2f;
                return;
            }
            if (!left && mogPlayer.rightCrescentMoonShots <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item17 with { PitchVariance = 0.2f }, Owner.Center);
                if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 2f;
                return;
            }

            SoundEngine.PlaySound(SoundID.Item41, Owner.Center);
            Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, Vector2.Zero, 100, Color.BlanchedAlmond, Main.rand.NextFloat(0.8f, 1.2f));
            for (int i = 0; i <= 12; i++)
            {
                Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, (shootVelocity * Main.rand.NextFloat(0.2f, 1.1f)).RotatedByRandom(0.4f), 0, default);
                dust2.noGravity = true;
                dust2.scale = Main.rand.NextFloat(0.8f, 1.4f);
            }
            if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 15f; // visual recoil effect
            Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _, true);
            if (Main.myPlayer == Projectile.owner)
            {
                // reduce ammo by 1
                if (left && mogPlayer.leftCrescentMoonShots > 0) mogPlayer.leftCrescentMoonShots--;
                if (!left && mogPlayer.rightCrescentMoonShots > 0) mogPlayer.rightCrescentMoonShots--;
                var source = Projectile.GetSource_FromThis();
                int type = ammo;
                if (ammo == ProjectileID.Bullet)
                {
                    type = ModContent.ProjectileType<CrescentMoonBullet>();
                    bulletDamage = (int)(bulletDamage * 1.3f);
                    knockback *= 1.5f;
                }
                Vector2 shootPos = Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.25f;
                Projectile.NewProjectile(source, shootPos, shootVelocity, type, bulletDamage, knockback, Projectile.owner);
                if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                {
                    string goreType = "RigGunCasing";
                    Vector2 spawnOffset = new(0, -11f);
                    Vector2 spawnPosition = Projectile.Center + (-Projectile.velocity * 5) + spawnOffset;
                    Gore.NewGore(Projectile.GetSource_FromAI(), spawnPosition, -shootVelocity * 2f, Mod.Find<ModGore>(goreType).Type);
                }
                CooldownTimer = attackTime / maxShots;
                ShootTimer = attackTime / 3;
            }
        }
    }
}