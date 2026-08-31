using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Config;
using MogMod.Items.Weapons.Ranged;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using Mono.Cecil;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class DreadsRetributionHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<DreadsRetribution>();
        public override float MaxOffsetLengthFromArm => 14f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -1f;
        public override float OffsetYDownwards => 5f;
        public int Time = 0;
        public int shotCounter = 0;
        public int framesBetweenShots = 0;
        public int maxShots = DreadsRetribution.maxShots;
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
            if (Time < DreadsRetribution.reloadTime)
            {
                if (Time == DreadsRetribution.reloadTime / 3)
                {
                    Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _);
                    SoundEngine.PlaySound(SoundID.Item23 with { Pitch = -0.1f }, Owner.Center);
                    SoundEngine.PlaySound(SoundID.Item108 with { Pitch = -0.2f }, Owner.Center);
                    if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 10f;
                }
            }
            if (Time >= DreadsRetribution.reloadTime)
            {
                if (framesBetweenShots == 0 && shotCounter <= maxShots + 1)
                {
                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 20;
                    SoundEngine.PlaySound(SoundID.Item5 with { Volume = 0.3f, Pitch = 0.05f, PitchVariance = 0.1f, MaxInstances = -1 }, Projectile.Center);
                    Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, Vector2.Zero, 100, Color.DeepSkyBlue, Main.rand.NextFloat(0.8f, 1.2f));
                    for (int i = 0; i <= 4; i++)
                    {
                        Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, (shootVelocity * Main.rand.NextFloat(0.2f, 1.1f)).RotatedByRandom(0.4f), 0, default);
                        dust2.noGravity = true;
                        dust2.scale = Main.rand.NextFloat(0.8f, 1.4f);
                    }
                    var attackSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
                    float cap = 5f;
                    if (attackSpeed > cap) attackSpeed = cap;
                    if (attackSpeed != 0f) attackSpeed = 1f / attackSpeed;
                    framesBetweenShots = (int)(5 * attackSpeed);
                    if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 2f; // visual recoil effect
                    Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _, true);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        var source = Projectile.GetSource_FromThis();
                        float tenthPi = 0.314159274f;
                        Vector2 arrowVel = shootVelocity;
                        arrowVel.Normalize();
                        arrowVel *= 50f;
                        bool arrowHitsTiles = Collision.CanHit(GunTipPosition, 0, 0, GunTipPosition + arrowVel, 0, 0);
                        int type = ammo;
                        if (ammo == ProjectileID.WoodenArrowFriendly) type = ModContent.ProjectileType<DreadsProj>();
                        for (int i = 0; i < 2; i++)
                        {
                            float piOffsetValue = (float)i - 0.4f;
                            Vector2 offsetSpawn = arrowVel.RotatedBy((double)(tenthPi * piOffsetValue), default);
                            if (!arrowHitsTiles) offsetSpawn -= arrowVel;
                            int arrowSpawn = Projectile.NewProjectile(source, GunTipPosition + offsetSpawn, shootVelocity, type, bulletDamage, knockback, Projectile.owner);
                            Main.projectile[arrowSpawn].noDropItem = true;
                        }
                    }
                    shotCounter++;
                    if (shotCounter == maxShots) framesBetweenShots = 18;
                }
                if (framesBetweenShots > 0) framesBetweenShots--;
            }
            if (shotCounter == maxShots && framesBetweenShots == 0)
            {
                Time = 2;
                shotCounter = 0;
            }
            Time++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Time < 2) return false;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);
            return false;
        }
    }
}