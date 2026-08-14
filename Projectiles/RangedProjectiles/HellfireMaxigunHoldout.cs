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
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    // code lifted from calamity mod spectralstormcannon
    public class HellfireMaxigunHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<HellfireMaxigun>();
        public override float MaxOffsetLengthFromArm => 24f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -1f;
        public override float OffsetYDownwards => 5f;
        public ref float Timer => ref Projectile.ai[0];
        private int BuiltHeat => (Owner.HeldItem.ModItem as HellfireMaxigun).BuiltUpHeat;
        private const int WarningTime = HellfireMaxigun.OverheatLevel - 100;
        public bool Overheating = false;
        public SlotId AudSlot;
        public static readonly SoundStyle WarningSound = new($"{nameof(MogMod)}/Sounds/SE/ArmletOn") { Volume = 1.1f, PitchVariance = .2f, MaxInstances = 0 };
        public override Vector2 GunTipPosition => Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.5f;
        public override void KillHoldoutLogic()
        {
            if (Owner.CantUseHoldout(false) || HeldItem.type != Owner.HeldItem.type || (BuiltHeat == 0 && !Main.mouseLeft))
            {
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();
                Projectile.Kill();
            }
        }
        public override void HoldoutAI()
        {
            if (Owner.MogMod().hellfireOverheat == 0) if (Main.mouseLeft) Timer++;
            else Timer = 0;

            // Once holding the fire button down long enough, start actually firing
            if (Timer >= 30)
            {
                // For some reason using HeldItem here breaks its functionality while being held on the cursor
                (Owner.HeldItem.ModItem as HellfireMaxigun).BuiltUpHeat++;

                // Overheat yourself if you fire too long
                if (BuiltHeat >= HellfireMaxigun.OverheatLevel)
                {
                    for (int e = 0; e < 7; e++)
                    {
                        Vector2 dustVel = -Projectile.rotation.ToRotationVector2().RotatedByRandom(MathHelper.Pi * 0.15f) * Main.rand.NextFloat(3.8f, 5.5f);
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Flare, dustVel, Scale: 1.5f);
                        dust.noGravity = true;
                    }

                    // Spectralstorm Cannon's overheat immediately resets heat after triggered
                    (Owner.HeldItem.ModItem as HellfireMaxigun).BuiltUpHeat = 1;
                    Owner.MogMod().hellfireOverheat = HellfireMaxigun.OverheatCooldown;
                    Overheating = true;
                    /*
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _);
                        Vector2 velocity = Main.rand.NextFloat(4f, 7.5f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition + Projectile.velocity * 5 + Main.rand.NextVector2Circular(7, 7), velocity.RotatedByRandom(MathHelper.ToRadians(4f)), ammo, (int)(Projectile.damage), Projectile.knockBack, Projectile.owner);
                        // minigun with a mag???
                        //if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                        //{
                        //    string goreType = "HellfireMag";
                        //    Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedBy(2f * -Owner.direction) * Main.rand.NextFloat(0.6f, 0.7f), Mod.Find<ModGore>(goreType).Type);
                        //}
                    }
                    */
                    return;
                }
                if (BuiltHeat == WarningTime) SoundEngine.PlaySound(WarningSound, Owner.Center);

                // Controls the escalating firing speed
                float firingLerp = Utils.GetLerpValue(0, 90, Timer - 30, true);
                int firingFrequency = (int)MathHelper.Lerp(HeldItem.useTime, HeldItem.useTime / 2, firingLerp);
                if (BuiltHeat >= WarningTime) firingFrequency = (int)MathHelper.Lerp(HeldItem.useTime, HeldItem.useTime / 4, firingLerp);
                // Actually fire shtuff
                if (Timer % firingFrequency == 0)
                {
                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 30;
                    Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, Vector2.Zero, 100, Color.OrangeRed, Main.rand.NextFloat(0.4f, 1.2f));
                    for (int i = 0; i <= 4; i++)
                    {
                        Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, (shootVelocity * Main.rand.NextFloat(0.8f, 1.6f)).RotatedByRandom(0.4f), 0, default);
                        dust2.noGravity = true;
                        dust2.scale = Main.rand.NextFloat(0.5f, 2.4f);
                    }
                    SoundEngine.PlaySound(SoundID.Item41 with { Volume = 0.3f, Pitch = 0.25f, PitchVariance = 0.1f, MaxInstances = -1 }, Projectile.Center);
                    OffsetLengthFromArm -= 2f;
                    Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity.RotatedByRandom(MathHelper.ToRadians(MathHelper.Lerp(0.2f, 3f, (Owner.HeldItem.ModItem as HellfireMaxigun).BuiltUpHeat * 0.01f))), ammo, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        MogModGlobalProjectile mogProj = proj.MogMod();
                        mogProj.fireBullet = true;
                        if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                        {
                            string goreType = "HellfireCasing";
                            Vector2 spawnOffset = new(0, -11f);
                            Vector2 spawnPosition = Projectile.Center + (-Projectile.velocity * 5) + spawnOffset;
                            Gore.NewGore(Projectile.GetSource_FromAI(), spawnPosition, -Projectile.velocity * 4f, Mod.Find<ModGore>(goreType).Type);
                        }
                    }
                }
            }
            if (BuiltHeat > 0)
            {
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                {
                    float heat = BuiltHeat * 0.01f;
                    float maxHeat = HellfireMaxigun.OverheatLevel * 0.01f;
                    ChargeSound.Position = Projectile.Center;
                    ChargeSound.Pitch = Utils.Remap(heat, 0, maxHeat, -0.4f, 0f);
                    ChargeSound.Volume = Utils.Remap(heat, 0, maxHeat, 0.4f, 1f) * 100;
                }
                else AudSlot = SoundEngine.PlaySound(SoundID.DD2_KoboldIgniteLoop with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
            }

            // Reset overheat draw color once the overheat ends
            if (Owner.MogMod().hellfireOverheat == 0)
            {
                Overheating = false;
                if (BuiltHeat <= 0)
                {
                    if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();
                }
            }
            // Draw smoke effect while overheated
            if (Overheating && Main.rand.NextBool(3))
            {
                Dust smoke = Dust.NewDustPerfect(GunTipPosition, DustID.Smoke, new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-2f, -5f)) * 7.5f, newColor: Color.WhiteSmoke, Scale: 1.55f);
                smoke.noGravity = true;
                smoke.fadeIn = 0.4f;
                smoke.scale *= 0.98f;
                smoke.color = Color.Lerp(Color.OrangeRed, Color.DarkGray, MathF.Abs(MathF.Sin(Owner.MogMod().hellfireOverheat * MathHelper.Pi / 30f)));
                if (Main.rand.NextBool(4)) smoke.scale *= 1.2f;
            }
            // Constantly move the warning sound on top of the player
            if (SoundEngine.TryGetActiveSound(AudSlot, out var warning) && warning.IsPlaying) warning.Position = Projectile.Center;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f) - (Owner.gravDir == -1 ? MathHelper.Pi * Owner.direction : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color tintColor = Overheating ? Color.Black : BuiltHeat >= WarningTime ? Color.Lerp(Color.OrangeRed, Color.White, MathF.Abs(MathF.Sin(Owner.miscCounter * MathHelper.Pi / 30f))) : Color.White;
            float opacity = Utils.GetLerpValue(0, WarningTime, BuiltHeat, true);

            for (int i = 0; i < 16; i++)
            {
                Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Projectiles/RangedProjectiles/HellfireMaxigunGhost").Value;
                Color auraColor = Color.OrangeRed * opacity * 0.6f;
                Vector2 drawOffset = ((MathHelper.TwoPi * i / 16f).ToRotationVector2() * 5);
                Main.EntitySpriteDraw(ghost, drawPosition + drawOffset, null, auraColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            }
            Main.EntitySpriteDraw(texture, drawPosition, null, tintColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            return false;
        }
    }
}