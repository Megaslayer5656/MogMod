using Microsoft.Xna.Framework;
using MogMod.Common.Config;
using MogMod.Items.Weapons.Ranged;
using MogMod.Projectiles.BaseProjectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class SinisterSpreaderHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<SinisterSpreader>();
        public override Vector2 GunTipPosition => Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.5f;
        public override float MaxOffsetLengthFromArm => 20f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -5f;
        public override float OffsetYDownwards => 5f;
        public ref float ShootTimer => ref Projectile.ai[0];
        public Color Colour1 = SinisterSpreader.MainColor1;
        public Color Colour2 = SinisterSpreader.MainColor2;
        public override void HoldoutAI()
        {
            var attackSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
            float cap = 20f;
            if (attackSpeed > cap) attackSpeed = cap;
            if (attackSpeed != 0f) attackSpeed = 1f / attackSpeed;
            if (ShootTimer == 0)
            {
                SoundEngine.PlaySound(SoundID.Item34, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item63 with { Volume = 0.8f, Pitch = Main.rand.NextFloat(-0.25f, -0.1f) }, Projectile.Center);
                Owner.PickAmmo(Owner.HeldItem, out _, out float shootSpeed, out int damage, out float knockback, out _);
                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < Main.rand.Next(10, 14); i++)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, (Projectile.velocity * 9).RotatedByRandom(0.15f), ModContent.ProjectileType<SinisterFire>(), damage, knockback, Projectile.owner);
                }
                ShootTimer = (int)(60 * attackSpeed);
                if (MogClientConfig.Instance.GunRecoil)
                    OffsetLengthFromArm -= 5f; // visual recoil effect
            }
            else
            {
                if (ShootTimer < (int)(60 * attackSpeed))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        float rotMulti = Main.rand.NextFloat(0.3f, 1f);
                        Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(5) ? DustID.FireworksRGB : DustID.RainbowTorch, newColor: Main.rand.NextBool() ? Colour2 : Colour1);
                        dust2.noGravity = true;
                        dust2.velocity = new Vector2(0, -2).RotatedByRandom(rotMulti * 0.3f) * (Main.rand.NextFloat(1f, 2.9f) - rotMulti);
                        dust2.scale = Main.rand.NextFloat(1.2f, 1.8f) * (Math.Abs(1f - (ShootTimer * 0.015f)));
                    }
                }
            }
            if (ShootTimer > 0) ShootTimer--;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            SoundEngine.PlaySound(SoundID.Item73 with { Volume = 0.7f }, Projectile.Center);
            ShootTimer = 100;
        }
    }
}