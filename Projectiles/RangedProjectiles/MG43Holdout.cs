using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Config;
using MogMod.Items.Weapons.Ranged;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class MG43Holdout : BaseGunHoldoutProjectile
    {
        // code lifted from calamity mod seadragon
        public override int AssociatedItemID => ModContent.ItemType<MG43MachineGun>();
        public override float MaxOffsetLengthFromArm => 24f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -1f;
        public override float OffsetYDownwards => 5f;
        public int Time = 0;
        public int shotCounter = 0;
        public int framesBetweenShots = 0;
        public bool transEffects = false;
        public int maxShots = MG43MachineGun.maxShots;
        public int rpm = MG43MachineGun.rpm;
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
            if (Time < MG43MachineGun.reloadTime)
            {
                if (Time == 2)
                {
                    if (Main.LocalPlayer.HasItemInAnyInventory(ItemID.GenderChangePotion)) transEffects = true;
                    else transEffects = Main.rand.NextBool(100);
                }
                if (Time == 30)
                {
                    if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                    {
                        string goreType = "RigGunMag";
                        Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedBy(2f * -Owner.direction) * Main.rand.NextFloat(0.6f, 0.7f), Mod.Find<ModGore>(goreType).Type);
                    }
                }
                if (Time % 60 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item149 with { Pitch = -0.1f }, Owner.Center);
                    SoundEngine.PlaySound(SoundID.Item108 with { Pitch = -0.2f }, Owner.Center);
                }
            }
            if (Time >= MG43MachineGun.reloadTime)
            {
                if (framesBetweenShots == 0 && shotCounter <= maxShots + 1)
                {
                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 30;
                    SoundEngine.PlaySound(SoundID.Item40 with { Volume = 0.3f, Pitch = 0.25f, PitchVariance = 0.1f, MaxInstances = -1 }, Projectile.Center);
                    Color color = Color.Lerp(new(91, 206, 250), new(245, 169, 184), MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f);
                    Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, Vector2.Zero, 100, transEffects ? color : Color.Goldenrod, Main.rand.NextFloat(0.8f, 1.2f));
                    for (int i = 0; i <= 4; i++)
                    {
                        Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, (shootVelocity * Main.rand.NextFloat(0.2f, 1.1f)).RotatedByRandom(0.4f), 0, transEffects ? color : default);
                        dust2.noGravity = true;
                        dust2.scale = Main.rand.NextFloat(0.8f, 1.4f);
                    }
                    var attackSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
                    float cap = 5f;
                    if (attackSpeed > cap) attackSpeed = cap;
                    if (attackSpeed != 0f) attackSpeed = 1f / attackSpeed;
                    framesBetweenShots = (int)(5 * attackSpeed);
                    OffsetLengthFromArm -= 2f;
                    Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        // different spread for each rpm
                        int fireRate = rpm;
                        if (rpm == 3) fireRate = Main.zenithWorld ? 0 : 5;
                        else if (rpm == 2) fireRate = Main.zenithWorld ? 0 : 20;
                        else fireRate = Main.zenithWorld ? 500 : 60;

                        // spread
                        float SpeedX = shootVelocity.X + Main.rand.Next(-fireRate, fireRate + 1) * 0.05f;
                        float SpeedY = shootVelocity.Y + Main.rand.Next(-fireRate, fireRate + 1) * 0.05f;
                        Vector2 newVelocity = new(SpeedX, SpeedY);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, newVelocity, ammo, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                        {
                            string goreType = "RigGunCasing";
                            Vector2 spawnOffset = new(0, -11f);
                            Vector2 spawnPosition = Projectile.Center + (-Projectile.velocity * 5) + spawnOffset;
                            Gore.NewGore(Projectile.GetSource_FromAI(), spawnPosition, -Projectile.velocity * 4f, Mod.Find<ModGore>(goreType).Type);
                        }
                    }
                    shotCounter++;
                    if (shotCounter == maxShots) framesBetweenShots = 18;
                }
                if (framesBetweenShots > 0) framesBetweenShots--;
            }
            if (shotCounter == maxShots && framesBetweenShots == 0)
            {
                //Reset all variables to allow left click to be held down
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

            if (transEffects)
            {
                Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Projectiles/RangedProjectiles/MG43Ghost").Value;
                float outlineWidth = 4;
                for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.25f)
                {
                    Main.spriteBatch.Draw(ghost,
                        drawPosition + new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX.RotatedBy(i + Projectile.rotation) * outlineWidth * Projectile.scale,
                        null,
                        Color.Lerp(new(91, 206, 250), new(245, 169, 184), MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f),
                        drawRotation,
                        rotationPoint,
                        Projectile.scale * Owner.gravDir,
                        flipSprite,
                        0
                    );
                }
            }
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);
            return false;
        }
    }
}