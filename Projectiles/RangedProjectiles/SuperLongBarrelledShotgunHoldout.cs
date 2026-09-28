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
    // projectile lifetime code lifted from calamity mod starfleet
    public class SuperLongBarrelledShotgunHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<SuperLongBarrelledShotgun>();
        public override float MaxOffsetLengthFromArm => 69f;
        public override float BaseOffsetY => -3f;
        public override float OffsetYDownwards => 4f;
        public ref float ShootTimer => ref Projectile.ai[0];
        public ref float ReloadTimer => ref Projectile.ai[1];
        public ref float LastUseTime => ref Projectile.ai[2];
        public int Cap = 10;
        public int shootTime = SuperLongBarrelledShotgun.reloadTime;
        public int attackTime = 0;
        public int maxShots = SuperLongBarrelledShotgun.maxShots;
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

            if ((Owner.HeldItem.type != ModContent.ItemType<SuperLongBarrelledShotgun>() && doingNothing) || (doingNothing && (Main.mapFullscreen || Owner.mouseInterface)) || Owner.dead)
            {
                Projectile.Kill();
                return;
            }

            bool hasAmmo = Owner.PickAmmo(HeldItem, out _, out _, out _, out _, out _, true);
            bool leftShootChecks = Owner.whoAmI == Main.myPlayer && (Main.mouseLeft && Main.mouseLeftRelease && !Main.mapFullscreen && !Owner.mouseInterface && ReloadTimer <= 0) && hasAmmo;
            bool rightShootChecks = Owner.whoAmI == Main.myPlayer && (Owner.MogMod().mouseRight && Main.mouseRightRelease && !Main.mapFullscreen && !Owner.mouseInterface && ReloadTimer <= 0) && hasAmmo;

            // if we ran out of ammo, reload
            if (mogPlayer.longBarrelShotgunShots != maxShots && KeybindSystem.FirstWeaponKeybind.Current && hasAmmo || ReloadTimer != 0)
            {
                ShootTimer = attackTime / maxShots;
                ReloadTimer++;
                if (ReloadTimer == 2)
                {
                    Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _);
                    SoundEngine.PlaySound(SoundID.Item149 with { Pitch = -0.2f }, Owner.Center);
                    SoundEngine.PlaySound(SoundID.Item108 with { Pitch = -0.3f }, Owner.Center);
                    if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 5f;
                }
                if (ReloadTimer == (attackTime / maxShots))
                {
                    if (mogPlayer.longBarrelShotgunShots < maxShots) mogPlayer.longBarrelShotgunShots++;
                    ReloadTimer = 0;
                }
                if (mogPlayer.longBarrelShotgunShots == maxShots)
                {
                    SoundEngine.PlaySound(SoundID.ResearchComplete with { Volume = 0.35f, Pitch = -0.3f }, Owner.Center);

                    int totalDusts = 5;
                    float starAngle = MathHelper.Pi / totalDusts;
                    for (int i = 0; i < totalDusts; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB);
                        Vector2 vel = (MathHelper.TwoPi * i / totalDusts).ToRotationVector2().RotatedBy(starAngle) * 3f;
                        Dust dust2 = Dust.NewDustPerfect(GunTipPosition, DustID.FireworksRGB, vel, 80, Color.SandyBrown, 1.2f);
                        dust2.noGravity = true;
                    }
                }
            }
            else
            {
                if (rightShootChecks) Shoot(shootVelocity, true);
                else if (leftShootChecks) Shoot(shootVelocity, false);
            }
            if (ShootTimer > 0) ShootTimer--;
        }
        public void Shoot(Vector2 shootVelocity, bool rightClicked)
        {
            MogPlayer mogPlayer = Owner.MogMod();
            if (mogPlayer.longBarrelShotgunShots <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item17 with { PitchVariance = 0.2f }, Owner.Center);
                if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 2f;
                return;
            }
            if (rightClicked)
            {
                while (mogPlayer.longBarrelShotgunShots > 0)
                {
                    ShootBullets(shootVelocity, 0.8f);
                }
            }
            else ShootBullets(shootVelocity);
        }
        public void ShootBullets(Vector2 shootVelocity, float spread = 0.15f)
        {
            MogPlayer mogPlayer = Owner.MogMod();
            SoundEngine.PlaySound(SoundID.Item38, Owner.Center);
            Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, Vector2.Zero, 100, Color.BlanchedAlmond, Main.rand.NextFloat(0.8f, 1.2f));
            for (int i = 0; i <= 12; i++)
            {
                Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, (shootVelocity * Main.rand.NextFloat(0.2f, 1.1f)).RotatedByRandom(0.4f), 0, default);
                dust2.noGravity = true;
                dust2.scale = Main.rand.NextFloat(0.8f, 1.4f);
            }
            if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 35f; // visual recoil effect
            Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _, true);
            if (Main.myPlayer == Projectile.owner)
            {
                // reduce ammo by 1
                if (mogPlayer.longBarrelShotgunShots > 0) mogPlayer.longBarrelShotgunShots--;
                var source = Projectile.GetSource_FromThis();
                int type = ammo;

                Owner.velocity += shootVelocity.SafeNormalize(Vector2.UnitX) * -8f;

                int bulletAmt = 4;
                for (int index = 0; index < bulletAmt; ++index)
                {
                    Projectile.NewProjectile(source, GunTipPosition, shootVelocity.RotatedByRandom(spread), type, bulletDamage, knockback, Projectile.owner);
                }
                if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                {
                    string goreType = "RigGunCasing";
                    Vector2 spawnOffset = new(0, -41f);
                    Vector2 spawnPosition = Projectile.Center + (-Projectile.velocity * 4f) + spawnOffset;
                    Gore.NewGore(Projectile.GetSource_FromAI(), spawnPosition, -shootVelocity.RotatedByRandom(spread), Mod.Find<ModGore>(goreType).Type);
                }
            }
        }
    }
}