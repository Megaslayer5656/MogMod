using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Config;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
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
using Terraria.WorldBuilding;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class CrescentMoonHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<CrescentMoon>();
        public override string Texture => "MogMod/Projectiles/RangedProjectiles/CrescentMoonHoldout";
        public static readonly SoundStyle StrongCharge = new($"{nameof(MogMod)}/Sounds/SE/bowChargeStrong") { Volume = 1.1f, PitchVariance = .2f, MaxInstances = 2 };
        public override float MaxOffsetLengthFromArm => 30f;
        public override float BaseOffsetY => -7f;
        public override float OffsetYDownwards => 3f;
        public ref float ShootTimer => ref Projectile.ai[0];
        public ref float ReloadTimer => ref Projectile.ai[1];
        public int LastUseTime = 0;
        public int CooldownTimer = 0;
        public int Cap = 5;
        public int shootTime = CrescentMoon.reloadTime;
        public int attackTime = 0;
        public int maxShots = CrescentMoon.maxShots;
        public int maxBarrageShots = CrescentMoon.maxBarrageShots;
        public bool Barraging = false;
        public bool playedChargeSound = false;
        public Color LeftColor = Color.SeaGreen;
        public Color RightColor = Color.Turquoise;
        public SlotId AudSlot;
        public override Vector2 GunTipPosition => Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.5f;
        public override void KillHoldoutLogic() { }
        public override void SendExtraAIHoldout(BinaryWriter writer)
        {
            writer.Write(LastUseTime);
            writer.Write(CooldownTimer);
            writer.Write(Projectile.spriteDirection);
            writer.Write(Barraging);
        }
        public override void ReceiveExtraAIHoldout(BinaryReader reader)
        {
            LastUseTime = reader.ReadInt32();
            CooldownTimer = reader.ReadInt32();
            Projectile.spriteDirection = reader.ReadInt32();
            Barraging = reader.ReadBoolean();
        }
        public override void HoldoutAI()
        {
            ModifyArmPosition = false;

            // right gun
            if (Projectile.ai[2] == 1) ArmPosition = Owner.RotatedRelativePoint(Owner.MountedCenter + new Vector2(-5 * Owner.direction, 3), true);
            // left gun
            else ArmPosition = Owner.RotatedRelativePoint(Owner.MountedCenter + new Vector2(5 * Owner.direction, -3), true);

            MogPlayer mogPlayer = Owner.MogMod();
            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 20;
            var attackSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
            if (attackSpeed > Cap) attackSpeed = Cap;
            if (attackSpeed != 0f) attackSpeed = 1f / attackSpeed;
            attackTime = (int)(shootTime * attackSpeed);

            SetUsage = false;
            bool doingNothing = (ReloadTimer == 0f && !Barraging && CooldownTimer <= 0f);
            if (LastUseTime == 0 || doingNothing) LastUseTime = Owner.HeldItem.useAnimation;
            if (!doingNothing) Owner.itemTime = Owner.itemAnimation = 5;

            if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound) && ChargeSound.IsPlaying && mogPlayer.crescentMoonPower >= 0.25f)
            {
                ChargeSound.Position = Projectile.Center;
                ChargeSound.Pitch = Utils.Remap(mogPlayer.crescentMoonPower, 0, 1f, -0.4f, 0f);
                ChargeSound.Volume = Utils.Remap(mogPlayer.crescentMoonPower, 0, 1f, 0f, 0.5f) * 100;
            }
            else if (mogPlayer.crescentMoonPower < 1f)
            {
                playedChargeSound = false;
                if (mogPlayer.crescentMoonPower < 0.25f) ChargeSound?.Stop();
                AudSlot = SoundEngine.PlaySound(SoundID.DD2_EtherianPortalIdleLoop with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
            }

            if ((Owner.HeldItem.type != ModContent.ItemType<CrescentMoon>() && doingNothing) || (doingNothing && (Main.mapFullscreen || Owner.mouseInterface)) || Owner.dead)
            {
                ChargeSound?.Stop();
                Projectile.Kill();
                return;
            }

            bool hasAmmo = Owner.PickAmmo(HeldItem, out _, out _, out _, out _, out _, true);
            bool leftShootChecks = Owner.whoAmI == Main.myPlayer && (Main.mouseLeft && !Main.mapFullscreen && !Owner.mouseInterface && ShootTimer <= 0 && ReloadTimer <= 0 && Projectile.ai[2] == 1f) && hasAmmo;
            bool rightShootChecks = Owner.whoAmI == Main.myPlayer && (mogPlayer.mouseRight && !Main.mapFullscreen && !Owner.mouseInterface && ShootTimer <= 0 && ReloadTimer <= 0 && Projectile.ai[2] == 0f) && hasAmmo;
            bool specialShootChecks = Owner.whoAmI == Main.myPlayer && (KeybindSystem.SecondWeaponKeybind.JustPressed && !Main.mapFullscreen && !Owner.mouseInterface && mogPlayer.crescentMoonPower >= 1f && ShootTimer <= 0 && ReloadTimer <= 0) || (Barraging && CooldownTimer > 0);

            bool noLeftAmmo = mogPlayer.leftCrescentMoonShots < maxShots && Projectile.ai[2] == 0f;
            bool noRightAmmo = mogPlayer.rightCrescentMoonShots < maxShots && Projectile.ai[2] == 1f;

            // if we ran out of ammo, reload
            if (((noLeftAmmo || noRightAmmo) && KeybindSystem.FirstWeaponKeybind.Current && hasAmmo && !Barraging && CooldownTimer <= 0f) || ReloadTimer != 0)
            {
                ShootTimer = attackTime / maxShots;
                ReloadTimer++;
                if (ReloadTimer <= (attackTime / maxShots))
                {
                    if (ReloadTimer == 2)
                    {
                        if (noLeftAmmo) mogPlayer.leftCrescentMoonShots++;
                        if (noRightAmmo) mogPlayer.rightCrescentMoonShots++;
                        Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _);
                        SoundEngine.PlaySound(SoundID.Item149 with { Pitch = -0.2f }, Owner.Center);
                        SoundEngine.PlaySound(SoundID.Item108 with { Pitch = -0.3f }, Owner.Center);
                        if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 4f;
                    }
                    if (ReloadTimer >= (attackTime / maxShots))
                    {
                        if ((Projectile.ai[2] == 0f && mogPlayer.leftCrescentMoonShots >= maxShots) || (Projectile.ai[2] == 1f && mogPlayer.rightCrescentMoonShots >= maxShots))
                        {
                            SoundEngine.PlaySound(SoundID.ResearchComplete with { Volume = 0.35f, Pitch = -0.3f }, Owner.Center);

                            int totalDusts = 5;
                            float starAngle = MathHelper.Pi / totalDusts;
                            for (int i = 0; i < totalDusts; i++)
                            {
                                Dust chargefull = Dust.NewDustPerfect(GunTipPosition, DustID.FireworksRGB);
                                Vector2 vel = (MathHelper.TwoPi * i / totalDusts).ToRotationVector2().RotatedBy(starAngle) * 2f;
                                Dust dust2 = Dust.NewDustPerfect(GunTipPosition, DustID.FireworksRGB, vel, 80, Projectile.ai[2] == 1f ? RightColor : LeftColor, 1.2f);
                                dust2.noGravity = true;
                            }
                        }
                        ReloadTimer = 0;
                    }
                }
            }
            else
            {
                if (specialShootChecks) BulletBarrage(shootVelocity, Projectile.ai[2]);
                else if (leftShootChecks) Shoot(shootVelocity, Projectile.ai[2]);
                else if (rightShootChecks) Shoot(shootVelocity, Projectile.ai[2]);
            }
            if (mogPlayer.crescentMoonPower >= 1f && !playedChargeSound && Projectile.ai[2] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item20 with { Pitch = -0.2f }, Owner.Center);
                for (int i = 0; i < 75; i++)
                {
                    float colorRando = Main.rand.NextFloat(0, 1);
                    float offsetAngle = MathHelper.TwoPi * i / 75f;
                    float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 3D);
                    float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 3D);

                    Vector2 vel = new Vector2(unitOffsetX, unitOffsetY) * 2.5f;
                    Dust charged = Dust.NewDustPerfect(Owner.Center, 267, vel);
                    charged.scale = 0.75f;
                    charged.fadeIn = 0.5f;
                    charged.color = Color.Lerp(LeftColor, RightColor, colorRando);
                    charged.noGravity = true;
                }
                playedChargeSound = true;
            }
            if (ShootTimer > 0) ShootTimer--;
            if (CooldownTimer > 0 && !Barraging) CooldownTimer--;
        }
        public void Shoot(Vector2 shootVelocity, float leftClick)
        {
            bool left = leftClick == 0f;
            MogPlayer mogPlayer = Owner.MogMod();
            if (left && mogPlayer.leftCrescentMoonShots <= 0)
            {
                if (mogPlayer.mouseRight && Main.mouseRightRelease)
                {
                    SoundEngine.PlaySound(SoundID.Item17 with { PitchVariance = 0.2f }, Owner.Center);
                    if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 2f;
                }
                return;
            }
            if (!left && mogPlayer.rightCrescentMoonShots <= 0)
            {
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    SoundEngine.PlaySound(SoundID.Item17 with { PitchVariance = 0.2f }, Owner.Center);
                    if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 2f;
                }
                return;
            }

            SoundEngine.PlaySound(SoundID.Item41, Owner.Center);
            Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, Vector2.Zero, 100, Color.BlanchedAlmond, Main.rand.NextFloat(0.8f, 1.2f));
            for (int i = 0; i <= 12; i++)
            {
                Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, (shootVelocity * Main.rand.NextFloat(0.2f, 1.1f)).RotatedByRandom(0.4f), 0, default);
                dust2.noGravity = true;
                dust2.scale = Main.rand.NextFloat(0.8f, 1.4f);
            }
            if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 15f; // visual recoil effect
            if (Main.myPlayer == Projectile.owner)
            {
                Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _, true);
                // reduce ammo by 1
                if (left && mogPlayer.leftCrescentMoonShots > 0) mogPlayer.leftCrescentMoonShots--;
                if (!left && mogPlayer.rightCrescentMoonShots > 0) mogPlayer.rightCrescentMoonShots--;
                var source = Projectile.GetSource_FromThis();
                int type = type = ModContent.ProjectileType<CrescentMoonBullet>();
                Vector2 shootPos = Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.25f;
                Projectile.NewProjectile(source, shootPos, shootVelocity, type, (int)(bulletDamage * (Owner.MogMod().crescentMoonPower + 0.5f)), knockback, Projectile.owner, ai1: 5f, ai2: Projectile.ai[2]);
                if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                {
                    string goreType = "RigGunCasing";
                    Vector2 spawnOffset = new(0, -11f);
                    Vector2 spawnPosition = Projectile.Center + (-Projectile.velocity * 5) + spawnOffset;
                    Gore.NewGore(Projectile.GetSource_FromAI(), spawnPosition, -shootVelocity * 0.8f, Mod.Find<ModGore>(goreType).Type);
                }
                CooldownTimer = attackTime / maxShots;
                ShootTimer = attackTime / 3;
            }
        }
        public void BulletBarrage(Vector2 shootVelocity, float bulletType)
        {
            Barraging = true;
            int waitTime = 48;
            MogPlayer mogPlayer = Owner.MogMod();
            if (CooldownTimer <= waitTime && Projectile.owner == Main.myPlayer && bulletType == 0)
            {
                bool cooldownCheck1 = CooldownTimer == 0;
                bool cooldownCheck2 = CooldownTimer == (waitTime / 2);
                bool cooldownCheck3 = CooldownTimer == waitTime;
                if (cooldownCheck1 || cooldownCheck2 || cooldownCheck3)
                {
                    Rectangle r = new((int)Owner.Hitbox.X, (int)Owner.Hitbox.Y - 50, Owner.Hitbox.Width, Owner.Hitbox.Height);
                    string text = "";
                    if (Main.zenithWorld)
                    {
                        text = cooldownCheck3 ? MiscUtils.GetTextFromModItem<CrescentMoon>("UltTextGFB3").ToString() 
                            : cooldownCheck2 ? MiscUtils.GetTextFromModItem<CrescentMoon>("UltTextGFB2").ToString() 
                            : MiscUtils.GetTextFromModItem<CrescentMoon>("UltTextGFB1").ToString();
                    }
                    else
                    {
                        text = cooldownCheck3 ? MiscUtils.GetTextFromModItem<CrescentMoon>("UltText3").ToString() 
                            : cooldownCheck2 ? MiscUtils.GetTextFromModItem<CrescentMoon>("UltText2").ToString() 
                            : MiscUtils.GetTextFromModItem<CrescentMoon>("UltText1").ToString();
                    }
                    float increment = cooldownCheck3 ? 10f : cooldownCheck2 ? 20f : 30f;
                    CombatText.NewText(r, (Color.Lerp(LeftColor, RightColor, MathF.Abs(MathF.Sin(Owner.miscCounter * MathHelper.Pi / increment)))), text, true);

                    SoundStyle voice = Owner.Male ? SoundID.PlayerHit : SoundID.FemaleHit;
                    SoundEngine.PlaySound(voice with { PitchVariance = 0.3f, MaxInstances = -1 }, Owner.Center);
                }
            }
            if (CooldownTimer <= ((maxBarrageShots * 2) + waitTime)) CooldownTimer++;
            else
            {
                Barraging = false;
                CooldownTimer = 0;
                mogPlayer.crescentMoonPower = 0f;
                mogPlayer.leftCrescentMoonShots = maxShots;
                mogPlayer.rightCrescentMoonShots = maxShots;
                return;
            }
            if (CooldownTimer < waitTime || CooldownTimer % 4 != (bulletType * 2)) return;
            SoundEngine.PlaySound(SoundID.Item41 with { Volume = 0.7f, MaxInstances = -1 }, Owner.Center);
            SoundEngine.PlaySound(SoundID.Item96 with { Volume = 0.6f, Pitch = 0.3f, MaxInstances = -1 }, Owner.Center);
            Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, Vector2.Zero, 100, Color.BlanchedAlmond, Main.rand.NextFloat(0.8f, 1.2f));
            for (int i = 0; i <= 12; i++)
            {
                Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustID.FireworksRGB : 303, (shootVelocity * Main.rand.NextFloat(0.2f, 1.1f)).RotatedByRandom(0.4f), 0, default);
                dust2.noGravity = true;
                dust2.scale = Main.rand.NextFloat(0.8f, 1.4f);
            }
            if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= 8f; // visual recoil effect
            if (Main.myPlayer == Projectile.owner)
            {
                Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _);
                var source = Projectile.GetSource_FromThis();
                int type = type = ModContent.ProjectileType<CrescentMoonBullet>();
                Vector2 shootPos = Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.25f;
                Projectile.NewProjectile(source, shootPos, shootVelocity, type, (int)(bulletDamage * (Owner.MogMod().crescentMoonPower + 0.5f)), knockback, Projectile.owner, ai2: bulletType);
                if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                {
                    string goreType = "RigGunCasing";
                    Vector2 spawnOffset = new(0, -11f);
                    Vector2 spawnPosition = Projectile.Center + (-Projectile.velocity * 5) + spawnOffset;
                    Gore.NewGore(Projectile.GetSource_FromAI(), spawnPosition, -shootVelocity * 0.8f, Mod.Find<ModGore>(goreType).Type);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            MogPlayer mogPlayer = Owner.MogMod();

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f) - (Owner.gravDir == -1 ? MathHelper.Pi * Owner.direction : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Color auraColor = Color.White;

            auraColor = (Color.Lerp(Projectile.ai[2] == 1f ? RightColor : LeftColor, Projectile.ai[2] == 1f ? LeftColor : RightColor, MathF.Abs(MathF.Sin(Owner.miscCounter * MathHelper.Pi / 30f)))) * (2f * mogPlayer.crescentMoonPower);

            if (mogPlayer.crescentMoonPower >= 0.5f || Barraging)
            {
                if ((mogPlayer.crescentMoonPower >= 0.75f || Barraging) && MogClientConfig.Instance.GunRecoil)
                {
                    float rumble = (Barraging ? 2f : mogPlayer.crescentMoonPower >= 1f ? 1.5f : (mogPlayer.crescentMoonPower * 0.5f));
                    drawPosition += Main.rand.NextVector2Circular(rumble, rumble);
                    //Main.NewText($"{rumble}, {drawPosition}", auraColor);
                }
                for (int i = 0; i < 16; i++)
                {
                    Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/CrescentMoonGhost").Value;
                    Vector2 drawOffset = ((MathHelper.TwoPi * i / 16f).ToRotationVector2() * (2f * mogPlayer.crescentMoonPower));
                    Main.EntitySpriteDraw(ghost, drawPosition + drawOffset, null, Projectile.GetAlpha(auraColor), drawRotation, rotationPoint, Projectile.scale, flipSprite);
                }
            }
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale, flipSprite);
            return false;
        }
    }
}