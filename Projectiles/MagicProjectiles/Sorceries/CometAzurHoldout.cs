using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Classes;
using MogMod.Items.Ammo.SorcerySpells.Legendary;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class CometAzurHoldout : BaseHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<CometAzur>();
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        public static readonly SoundStyle laserSound = new("Terraria/Sounds/Item_15")
        {
            Volume = 1f,
            PitchVariance = 0.2f,
            MaxInstances = -1
        };
        public override float HoldoutOffset => Projectile.width / 2;
        public override float TurnSpeed => 0.03f;
        public override int HoldoutHandling => HoldoutStyle.Rigid;
        public override int SyncTimer => 1;
        public ref float Timer => ref Projectile.ai[0];
        public ref float CanShoot => ref Projectile.ai[2];
        public static Color WeakColor => CometAzur.WeakColor;
        public static Color StrongColor => CometAzur.StrongColor;
        //public SlotId AudSlot;
        bool coolingDown = false; // cooldown effects
        int MinCharge = 60; // how long before firing proj
        int Cap = 30; // to prevent being unable to fire with enough attack speed
        public override void SetDefaults()
        {
            Projectile.width = Owner.HeldItem.width;
            Projectile.height = Owner.HeldItem.height;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;

            Projectile.netImportant = true;
        }
        public override void HoldoutAI()
        {
            var attackSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
            if (attackSpeed > Cap) attackSpeed = Cap;
            if (attackSpeed != 0f) attackSpeed = 1f / attackSpeed;

            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 15;

            Projectile.width = Owner.HeldItem.width;
            Projectile.height = Owner.HeldItem.height;
            Timer++;
            if (Timer == 3) Projectile.alpha = 0;

            bool canUseMana = Owner.CheckMana(Owner.HeldItem);
            if (Owner.CantUseHoldout() || !canUseMana)
            {
                CanShoot = 5f;
                //if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();
                Projectile.Kill();
                return;
            }
            else
            {
                Owner.itemRotation = ((Projectile.rotation + MathHelper.Pi).ToRotationVector2() * -Owner.direction).ToRotation();
                if (Timer >= MinCharge)
                {
                    Projectile.timeLeft = 2; // refresh holdout lifetime

                    // dust effect
                    int dustNum = 1;
                    for (int i = 0; i <= dustNum; i++)
                    {
                        Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 30f, Main.rand.NextBool(3) ? DustID.MagnetSphere : DustID.ApprenticeStorm, -(Projectile.velocity * Main.rand.NextFloat(-1.5f, 1.5f)).RotatedByRandom(1.4f));
                        dust2.noGravity = true;
                        dust2.scale = Main.rand.NextFloat(0.9f, 1.6f);
                        dust2.color = Color.Lerp(WeakColor, StrongColor, (float)Math.Abs(Math.Cos(Main.GlobalTimeWrappedHourly)));
                    }

                    Owner.PickAmmo(Owner.HeldItem, out _, out float speed, out int damage, out float knockback, out _, true);
                    if (Timer % 21 == 0) SoundEngine.PlaySound(laserSound, Projectile.Center);
                    if (Timer % 3 == 0)
                    {
                        Owner.CheckMana(Owner.HeldItem, -1, true);
                        int orb = ModContent.ProjectileType<CometAzurProj>();
                        float Spread = 0.12f;
                        Vector2 velocity = Projectile.velocity * 15;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - Projectile.velocity, velocity.RotatedByRandom(Spread), orb, damage, knockback, Projectile.owner);
                    }

                    var source = Projectile.GetSource_FromThis();
                    int type = ModContent.ProjectileType<CometAzurLaser>();
                    if (Projectile.owner == Main.myPlayer && Owner.ownedProjectileCounts[type] < 1)
                    {
                        SoundEngine.PlaySound(SoundID.Zombie104, Projectile.Center);
                        Vector2 spawnPos = Owner.MountedCenter;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, Projectile.velocity, type, damage, knockback, Projectile.owner, ai1: Projectile.whoAmI);
                    }/*
                    if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                    {
                        ChargeSound.Position = Projectile.Center;
                        ChargeSound.Pitch = Utils.Remap(ShootTimer, 0, 1f, -0.4f, 0f);
                        ChargeSound.Volume = Utils.Remap(ShootTimer, 0, 1f, 0f, 0.75f) * 100;
                    }
                    else if (ShootTimer < MinCharge * attackSpeed) AudSlot = SoundEngine.PlaySound(SoundID.DD2_EtherianPortalIdleLoop with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
                    */
                }
                else if (Timer == 2) SoundEngine.PlaySound(SoundID.Item13, Projectile.Center);
            }
        }
    }
}