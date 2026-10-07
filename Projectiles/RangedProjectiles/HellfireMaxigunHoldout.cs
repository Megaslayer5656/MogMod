using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Config;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Weapons.Ranged;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using ReLogic.Utilities;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class HellfireMaxigunHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<HellfireMaxigun>();
        public override Vector2 GunTipPosition => Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.5f;
        public override float MaxOffsetLengthFromArm => 24f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -1f;
        public override float OffsetYDownwards => 5f;

        private int BuiltHeat => Owner.MogMod().hellfireHeat;
        private const int WarningTime = HellfireMaxigun.OverheatLevel - 100;
        public int MaxHeat = HellfireMaxigun.OverheatLevel;
        public bool Overheating = false;
        public bool playedWarningSound = false;
        public static readonly SoundStyle WarningSound = new($"{nameof(MogMod)}/Sounds/SE/ArmletOn") { Volume = 1.1f, PitchVariance = .2f, MaxInstances = 0 };

        public ref float ShootTimer => ref Projectile.ai[0];
        public ref float ReloadTimer => ref Projectile.ai[1];
        public ref float LastUseTime => ref Projectile.ai[2];
        public int Cap = 10;
        public float MinShootSpeed = 5f;
        public float MaxShootSpeed = 9f;
        public int shootTime = 70;
        public int attackTime = 0;
        public SlotId AudSlot;
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
            bool doingNothing = ReloadTimer == 0 && BuiltHeat <= 0;
            if (LastUseTime == 0 || doingNothing) LastUseTime = Owner.HeldItem.useAnimation;
            if (!doingNothing) Owner.itemTime = Owner.itemAnimation = 5;

            if ((Owner.HeldItem.type != AssociatedItemID && doingNothing) || (doingNothing && (Main.mapFullscreen || Owner.mouseInterface)) || Owner.dead)
            {
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();   
                Projectile.Kill();
                return;
            }

            bool hasAmmo = Owner.PickAmmo(HeldItem, out _, out _, out _, out _, out _, true);
            bool leftShootChecks = Owner.whoAmI == Main.myPlayer && (Main.mouseLeft && !Main.mapFullscreen && !Owner.mouseInterface && ShootTimer <= 0 && ReloadTimer <= 0) && hasAmmo;

            // if the gun is not overheating, fire
            if (!Overheating)
            {
                if (leftShootChecks) Shoot(shootVelocity);
                if (BuiltHeat < MaxHeat && Main.mouseLeft)
                {
                    if (BuiltHeat < MaxHeat) mogPlayer.hellfireHeat += 1;
                }
                else if (BuiltHeat > 0 && ShootTimer <= 0 && !Main.mouseLeft) mogPlayer.hellfireHeat--;
            }
            else
            {
                if (BuiltHeat > 0) mogPlayer.hellfireHeat -= 1;
                else Overheating = false;
                // Draw smoke effect while overheated
                if (Main.rand.NextBool(3))
                {
                    Dust smoke = Dust.NewDustPerfect(GunTipPosition, DustID.Smoke, new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-2f, -5f)) * 7.5f, newColor: Color.WhiteSmoke, Scale: 1.55f);
                    smoke.noGravity = true;
                    smoke.fadeIn = 0.4f;
                    smoke.scale *= 0.98f;
                    smoke.color = Color.Lerp(Color.OrangeRed, Color.DarkGray, MathF.Abs(MathF.Sin(mogPlayer.hellfireHeat * MathHelper.Pi / 30f)));
                    if (Main.rand.NextBool(4)) smoke.scale *= 1.2f;
                }
                // Constantly move the warning sound on top of the player
                if (SoundEngine.TryGetActiveSound(AudSlot, out var warning) && warning.IsPlaying) warning.Position = Projectile.Center;
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
            else if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();
            if (ShootTimer > 0) ShootTimer--;
        }
        public void Shoot(Vector2 shootVelocity)
        {
            MogPlayer mogPlayer = Owner.MogMod();

            // Overheat yourself if you fire too long
            if (BuiltHeat >= MaxHeat)
            {
                for (int e = 0; e < 7; e++)
                {
                    Vector2 dustVel = -Projectile.rotation.ToRotationVector2().RotatedByRandom(MathHelper.Pi * 0.15f) * Main.rand.NextFloat(3.8f, 5.5f);
                    Dust overheatDust = Dust.NewDustPerfect(Projectile.Center, DustID.Flare, dustVel, Scale: 1.5f);
                    overheatDust.noGravity = true;
                }

                Overheating = true;
                return;
            }
            if (BuiltHeat >= WarningTime && !playedWarningSound) WarningEffect();
            if (BuiltHeat < WarningTime) playedWarningSound = false;

            Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, Vector2.Zero, 100, Color.OrangeRed, Main.rand.NextFloat(0.4f, 1.2f));
            for (int i = 0; i <= 4; i++)
            {
                Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, (shootVelocity * Main.rand.NextFloat(0.8f, 1.6f)).RotatedByRandom(0.4f), 0, default);
                dust2.noGravity = true;
                dust2.scale = Main.rand.NextFloat(0.5f, 2.4f);
            }
            SoundEngine.PlaySound(SoundID.Item41 with { Volume = 0.3f, Pitch = 0.25f, PitchVariance = 0.1f, MaxInstances = -1 }, Projectile.Center);
            if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 2f;
            Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _);
            Vector2 shootPos = Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.25f;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), shootPos, shootVelocity.RotatedByRandom(MathHelper.ToRadians(MathHelper.Lerp(0.2f, 3f, BuiltHeat * 0.01f))), ammo, Projectile.damage, Projectile.knockBack, Projectile.owner);
                MogModGlobalProjectile mogProj = proj.MogMod();
                mogProj.fireBullet = true;
                if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                {
                    string goreType = "HellfireCasing";
                    Vector2 spawnOffset = new(0, -11f);
                    Vector2 spawnPosition = Projectile.Center + (-Projectile.velocity * 5) + spawnOffset;
                    Gore.NewGore(Projectile.GetSource_FromAI(), spawnPosition, -Projectile.velocity * 4f, Mod.Find<ModGore>(goreType).Type);
                }
                float value = MathHelper.Lerp(MinShootSpeed, MaxShootSpeed, BuiltHeat * 0.01f);
                ShootTimer = (int)(attackTime / value);
            }
        }
        public void WarningEffect()
        {
            SoundEngine.PlaySound(WarningSound, Owner.Center);
            playedWarningSound = true;
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
                Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/HellfireMaxigunGhost").Value;
                Color auraColor = Color.OrangeRed * opacity * 0.6f;
                Vector2 drawOffset = ((MathHelper.TwoPi * i / 16f).ToRotationVector2() * 4);
                Main.EntitySpriteDraw(ghost, drawPosition + drawOffset, null, auraColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            }
            Main.EntitySpriteDraw(texture, drawPosition, null, tintColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            return false;
        }
    }
}