using Microsoft.Xna.Framework;
using MogMod.Common.Config;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Weapons.Ranged;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class MosinHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<Mosin>();
        public static readonly SoundStyle UseSound = new($"{nameof(MogMod)}/Sounds/SE/MosinShot") { Volume = .3f, PitchVariance = .02f };
        public override float MaxOffsetLengthFromArm => 24f;
        public override float BaseOffsetY => -3f;
        public override float OffsetYDownwards => 4f;
        public int Time = 0;
        public int ReloadTime = 0;
        public int framesBetweenShots = 0;
        public int shootTime = Mosin.reloadTime / 3;
        public int maxShots = Mosin.maxShots;
        public override Vector2 GunTipPosition => Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.5f;
        public override void KillHoldoutLogic()
        {
            if (Owner.CantUseHoldout() || HeldItem.type != AssociatedItemID)
            {
                Projectile.Kill();
            }
        }
        public override void HoldoutAI()
        {
            MogPlayer mogPlayer = Owner.MogMod();
            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 20;
            var attackSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
            float cap = 5f;
            if (attackSpeed > cap) attackSpeed = cap;
            if (attackSpeed != 0f) attackSpeed = 1f / attackSpeed;
            int attackTime = (int)(shootTime * attackSpeed);
            // if we ran out of ammo, reload
            if (mogPlayer.mosinShots == 0)
            {
                //Main.NewText($"{ReloadTime}, {shootTime}");
                ReloadTime++;
                if (ReloadTime == attackTime - 20)
                {
                    //Main.NewText($"loading ammo and mag gores", Color.AntiqueWhite);
                    if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                    {
                        string goreType = "RigGunMag";
                        Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, shootVelocity.RotatedBy(2f * -Owner.direction) * Main.rand.NextFloat(0.6f, 0.7f), Mod.Find<ModGore>(goreType).Type);
                    }
                    for (int i = 0; i < maxShots; i++) Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _);
                    SoundEngine.PlaySound(SoundID.Item149 with { Pitch = -0.2f }, Owner.Center);
                    SoundEngine.PlaySound(SoundID.Item108 with { Pitch = -0.3f }, Owner.Center);
                    if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 10f;
                }
                if (ReloadTime >= attackTime)
                {
                    //Main.NewText($"setting ammo to max shots", Color.Khaki);
                    mogPlayer.mosinShots = maxShots;
                }
            }
            // otherwise, add time
            else Time++;
            // if time is greater than shoot time, prepare fire
            if (Time >= attackTime + 30)
            {
                // reduce ammo by 1
                mogPlayer.mosinShots--;

                SoundEngine.PlaySound(UseSound, Owner.Center);
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
                    var source = Projectile.GetSource_FromThis();
                    int type = ammo;
                    if (ammo == ProjectileID.Bullet)
                    {
                        type = ModContent.ProjectileType<MosinLPSProj>();
                        bulletDamage = (int)(bulletDamage * 1.3f);
                        knockback *= 1.5f;
                    }
                    Owner.velocity += shootVelocity.SafeNormalize(Vector2.UnitX) * -6f;
                    Projectile.NewProjectile(source, GunTipPosition, shootVelocity, type, bulletDamage, knockback, Projectile.owner);
                    if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                    {
                        string goreType = "RigGunCasing";
                        Vector2 spawnOffset = new(0, -11f);
                        Vector2 spawnPosition = Projectile.Center + (-Projectile.velocity * 5) + spawnOffset;
                        Gore.NewGore(Projectile.GetSource_FromAI(), spawnPosition, -shootVelocity * 4f, Mod.Find<ModGore>(goreType).Type);
                    }
                }
                // reset the time so we can prepare to shoot again
                Time = 2;

                if (mogPlayer.mosinShots <= 0)
                {
                    ReloadTime = 2;
                    Time = 2;
                }
            }
        }
    }
}