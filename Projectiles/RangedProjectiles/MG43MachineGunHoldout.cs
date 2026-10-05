using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Config;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Weapons.Ranged;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class MG43MachineGunHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<MG43MachineGun>();
        public override float MaxOffsetLengthFromArm => 14f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -1f;
        public override float OffsetYDownwards => 5f;
        public override Vector2 GunTipPosition => Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.5f;
        public ref float ShootTimer => ref Projectile.ai[0];
        public ref float ReloadTimer => ref Projectile.ai[1];
        public ref float LastUseTime => ref Projectile.ai[2];

        public bool transEffects = false;
        public int maxShots = MG43MachineGun.maxShots;
        public int RPM => Owner.MogMod().mg43RPM;
        public int reloadTime = MG43MachineGun.reloadTime;
        public int attackTime = 0;

        public int Cap = 10;
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
            attackTime = (int)((reloadTime - maxShots) * attackSpeed);

            SetUsage = false;
            bool doingNothing = ReloadTimer == 0;
            if (LastUseTime == 0 || doingNothing) LastUseTime = Owner.HeldItem.useAnimation;
            if (!doingNothing) Owner.itemTime = Owner.itemAnimation = 5;

            if ((Owner.HeldItem.type != ModContent.ItemType<MG43MachineGun>() && doingNothing) || (doingNothing && (Main.mapFullscreen || Owner.mouseInterface)) || Owner.dead)
            {
                Projectile.Kill();
                return;
            }

            bool hasAmmo = Owner.PickAmmo(HeldItem, out _, out _, out _, out _, out _, true);
            bool leftShootChecks = Owner.whoAmI == Main.myPlayer && (Main.mouseLeft && !Main.mapFullscreen && !Owner.mouseInterface && ShootTimer <= 0 && ReloadTimer <= 0) && hasAmmo;
            bool specialShootChecks = Owner.whoAmI == Main.myPlayer && (KeybindSystem.SecondWeaponKeybind.JustPressed && !Main.mapFullscreen && !Owner.mouseInterface && ReloadTimer <= 0);

            // if we ran out of ammo, reload
            if (mogPlayer.mg43Shots != maxShots && KeybindSystem.FirstWeaponKeybind.JustPressed && hasAmmo || ReloadTimer != 0)
            {
                ReloadTimer++;
                if (ReloadTimer == 2 || ReloadTimer % 60 == 0 && ReloadTimer != reloadTime)
                {
                    if (ReloadTimer == 2)
                    {
                        if (Main.LocalPlayer.HasItemInAnyInventory(ItemID.GenderChangePotion)) transEffects = true;
                        else transEffects = Main.rand.NextBool(100);

                        if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                        {
                            string goreType = "RigGunMag";
                            Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedBy(2f * -Owner.direction) * Main.rand.NextFloat(0.6f, 0.7f), Mod.Find<ModGore>(goreType).Type);
                        }
                        Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _);
                    }
                    SoundEngine.PlaySound(SoundID.Item149 with { Pitch = -0.1f }, Owner.Center);
                    SoundEngine.PlaySound(SoundID.Item108 with { Pitch = -0.2f }, Owner.Center);
                    if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= ReloadTimer == 2 ? 4f : 2f;
                }
                if (ReloadTimer >= reloadTime)
                {
                    SoundEngine.PlaySound(SoundID.ResearchComplete with { Volume = 0.35f, Pitch = -0.3f }, Owner.Center);

                    Color NewColor(Color colour)
                    {
                        Color color = Color.Lerp(new(91, 206, 250), new(245, 169, 184), MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f);
                        return transEffects ? color : colour;
                    }
                    int totalDusts = 5;
                    float starAngle = MathHelper.Pi / totalDusts;
                    for (int i = 0; i < totalDusts; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB);
                        Vector2 vel = (MathHelper.TwoPi * i / totalDusts).ToRotationVector2().RotatedBy(starAngle) * 2f;
                        Dust dust2 = Dust.NewDustPerfect(GunTipPosition, DustID.FireworksRGB, vel, 80, NewColor(Color.LightGoldenrodYellow), 1.2f);
                        dust2.noGravity = true;
                    }

                    mogPlayer.mg43Shots = maxShots;
                    ReloadTimer = 0;
                }
            }
            else
            {
                if (leftShootChecks) Shoot(shootVelocity);
                else if (specialShootChecks)
                {
                    // determine RPM
                    SoundEngine.PlaySound(SoundID.Item149, Owner.MountedCenter);
                    if (RPM > 1) mogPlayer.mg43RPM--;
                    else mogPlayer.mg43RPM = 3;
                }
            }
            if (ShootTimer > 0) ShootTimer--;
        }
        public void Shoot(Vector2 shootVelocity)
        {
            MogPlayer mogPlayer = Owner.MogMod();
            if (mogPlayer.mg43Shots <= 0)
            {
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    SoundEngine.PlaySound(SoundID.Item17 with { PitchVariance = 0.2f }, Owner.Center);
                    if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 3f;
                }
                return;
            }

            SoundEngine.PlaySound(SoundID.Item40 with { Volume = 0.3f, Pitch = 0.25f, PitchVariance = 0.1f, MaxInstances = -1 }, Projectile.Center);
            Color NewColor(Color colour)
            {
                Color color = Color.Lerp(new(91, 206, 250), new(245, 169, 184), MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f);
                return transEffects ? color : colour;
            }
            Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, Vector2.Zero, 100, NewColor(Color.Goldenrod), Main.rand.NextFloat(0.8f, 1.2f));
            for (int i = 0; i <= 4; i++)
            {
                Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, (shootVelocity * Main.rand.NextFloat(0.2f, 1.1f)).RotatedByRandom(0.4f), 0, NewColor(Color.LightGray));
                dust2.noGravity = true;
                dust2.scale = Main.rand.NextFloat(0.8f, 1.4f);
            }
            if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 2f; // visual recoil effect
            Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _, true);
            if (Main.myPlayer == Projectile.owner)
            {
                // reduce ammo by 1
                if (mogPlayer.mg43Shots > 0) mogPlayer.mg43Shots--;

                var source = Projectile.GetSource_FromThis();

                // different spread for each RPM
                int fireRate = RPM;
                if (RPM == 3) fireRate = Main.zenithWorld ? 0 : 5;
                else if (RPM == 2) fireRate = Main.zenithWorld ? 0 : 20;
                else fireRate = Main.zenithWorld ? 500 : 60;

                // spread
                float SpeedX = shootVelocity.X + Main.rand.Next(-fireRate, fireRate + 1) * 0.05f;
                float SpeedY = shootVelocity.Y + Main.rand.Next(-fireRate, fireRate + 1) * 0.05f;
                Vector2 newVelocity = new(SpeedX, SpeedY);
                Vector2 shootPos = Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.25f;
                Projectile.NewProjectile(source, shootPos, newVelocity, ammo, bulletDamage, knockback, Projectile.owner);
                if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                {
                    string goreType = "RigGunCasing";
                    Vector2 spawnOffset = new(0, -11f);
                    Vector2 spawnPosition = Projectile.Center + (-Projectile.velocity * 5) + spawnOffset;
                    Gore.NewGore(Projectile.GetSource_FromAI(), spawnPosition, -Projectile.velocity * 4f, Mod.Find<ModGore>(goreType).Type);
                }

                ShootTimer = attackTime;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (transEffects)
            {
                Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/MG43Ghost").Value;
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