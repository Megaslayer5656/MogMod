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
    public class TerrablazerHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<Terrablazer>();
        public override Vector2 GunTipPosition => Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.5f;
        public override float MaxOffsetLengthFromArm => 20f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -5f;
        public override float OffsetYDownwards => 5f;
        public ref float ShootTimer => ref Projectile.ai[0];
        //public ref float ShotsFired => ref Projectile.ai[1];
        public Color Colour1 = Terrablazer.MainColor1;
        public Color Colour2 = Terrablazer.MainColor2;
        public override void HoldoutAI()
        {
            var attackSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
            float cap = 5f;
            if (attackSpeed > cap) attackSpeed = cap;
            if (attackSpeed != 0f) attackSpeed = 1f / attackSpeed;
            int cooldown = (int)(60 * attackSpeed);
            int attackCooldown = cooldown / 3;

            float rotMulti = Main.rand.NextFloat(0.3f, 1f);
            Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(5) ? DustID.FireworksRGB : DustID.RainbowTorch, newColor: Main.rand.NextBool() ? Colour2 : Colour1);
            dust2.noGravity = true;
            dust2.velocity = new Vector2(0, -2).RotatedByRandom(rotMulti * 0.3f) * (Main.rand.NextFloat(1f, 2.9f) - rotMulti);
            dust2.scale = Main.rand.NextFloat(1.2f, 1.8f) * ((ShootTimer > 60 ? 60 : ShootTimer) * 0.015f);
            if (ShootTimer > attackCooldown)
            {
                if (ShootTimer % (attackCooldown / 4) == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item34, Projectile.Center);
                    Owner.PickAmmo(Owner.HeldItem, out _, out float shootSpeed, out int damage, out float knockback, out _);
                    SoundEngine.PlaySound(SoundID.Item63 with { Volume = 0.8f, Pitch = Main.rand.NextFloat(-0.25f, -0.1f) }, Projectile.Center);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        //for (int i = 0; i < 2; i++)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, (Projectile.velocity * 9).RotatedByRandom(0.08f), ModContent.ProjectileType<TerraFire>(), damage, knockback, Projectile.owner);
                    }
                    if (MogClientConfig.Instance.GunRecoil)
                        OffsetLengthFromArm -= 1f; // visual recoil effect
                }
            }
            ShootTimer++;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            SoundEngine.PlaySound(SoundID.Item73 with { Volume = 0.7f }, Projectile.Center);
        }
    }
}