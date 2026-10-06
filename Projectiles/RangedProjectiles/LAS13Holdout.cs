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
    public class LAS13Holdout : BaseGunHoldoutProjectile
    {
        // holdout positioning
        public override int AssociatedItemID => ModContent.ItemType<LAS13Trident>();
        public override Vector2 GunTipPosition => Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.5f;
        public override float MaxOffsetLengthFromArm => 18f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -5f;
        public override float OffsetYDownwards => 5f;

        // heat management
        private int BuiltHeat => Owner.MogMod().las13Heat;
        private const int WarningTime = LAS13Trident.OverheatLevel - 70;
        public int MaxHeat = LAS13Trident.OverheatLevel;
        public bool RemindReload = false;
        public bool Overheating = false;
        public bool playedWarningSound = false;
        public static readonly SoundStyle WarningSound = new($"{nameof(MogMod)}/Sounds/SE/ArmletOn") { Volume = 1.1f, PitchVariance = .2f, MaxInstances = 0 };

        // shoot stats
        public int Cap = 10;
        public float ShootSpeed = LAS13Trident.ShootSpeed;
        public float MinHeatDamage = LAS13Trident.MinHeatDamage;
        public float MaxHeatDamage = LAS13Trident.MaxHeatDamage;
        public int shootTime = LAS13Trident.reloadTime - 60;
        public int attackTime = 0;

        // timers
        public ref float ShootTimer => ref Projectile.ai[0];
        public ref float ReloadTimer => ref Projectile.ai[1];
        public ref float LastUseTime => ref Projectile.ai[2];
        public int ReminderTimer = 0;

        public override void KillHoldoutLogic() { }
        public override void SendExtraAIHoldout(BinaryWriter writer)
        {
            writer.Write(Projectile.spriteDirection);
            writer.Write(ReminderTimer); // since ReminderTimer doesnt use an ai[] array it has to be synced manually
        }
        public override void ReceiveExtraAIHoldout(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadInt32();
            ReminderTimer = reader.ReadInt32();
        }
        public override void HoldoutAI()
        {
            // the player
            MogPlayer mogPlayer = Owner.MogMod();

            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 4;

            // determine attack speed manually since this is a projectile
            var attackSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
            if (attackSpeed > Cap) attackSpeed = Cap;
            if (attackSpeed != 0f) attackSpeed = 1f / attackSpeed;
            attackTime = (int)(shootTime * attackSpeed);

            // set to false since we want to be able to switch off the weapon
            SetUsage = false;
            // check if the weapon is not being used, and if so, allow the owner to switch off the weapon
            bool doingNothing = ReloadTimer == 0 && BuiltHeat <= 0;
            if (LastUseTime == 0 || doingNothing) LastUseTime = Owner.HeldItem.useAnimation;
            if (!doingNothing) Owner.itemTime = Owner.itemAnimation = 5;

            // kill the holdout if not being used
            if ((Owner.HeldItem.type != AssociatedItemID && doingNothing) || (doingNothing && (Main.mapFullscreen || Owner.mouseInterface)) || Owner.dead)
            {
                Projectile.Kill();
                return;
            }

            bool hasAmmo = Owner.PickAmmo(HeldItem, out _, out _, out _, out _, out _, true);
            // check if the owner is able to left click
            bool leftShootChecks = Owner.whoAmI == Main.myPlayer && (Main.mouseLeft && !Main.mapFullscreen && !Owner.mouseInterface && ShootTimer <= 0 && ReloadTimer <= 0) && hasAmmo;

            // if we overheated, reload
            if (Overheating && KeybindSystem.FirstWeaponKeybind.JustPressed && hasAmmo || ReloadTimer != 0)
            {
                ReloadTimer++;
                if (ReloadTimer == 2 || ReloadTimer == attackTime / 2)
                {
                    if (ReloadTimer == 2)
                    {
                        SoundEngine.PlaySound(SoundID.Item149 with { Pitch = -0.2f }, Owner.Center);
                        if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                        {
                            string goreType = "HellfireMag"; // TODO: change this
                            Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, shootVelocity.RotatedBy(2f * -Owner.direction) * Main.rand.NextFloat(0.6f, 0.7f), Mod.Find<ModGore>(goreType).Type);
                        }
                        // consume ammo
                        for (int i = 0; i < LAS13Trident.NumAmmoUsed; i++) Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _);
                    }
                    else SoundEngine.PlaySound(SoundID.Item5 with { Pitch = -0.3f }, Owner.Center);
                    SoundEngine.PlaySound(SoundID.Item108 with { Pitch = -0.3f }, Owner.Center);
                    // slight recoil to simulate reloading the holdout
                    if (MogClientConfig.Instance.GunRecoil) OffsetLengthFromArm -= ReloadTimer == 2 ? 4f : 2f;
                }
                if (ReloadTimer >= attackTime)
                {
                    SoundEngine.PlaySound(SoundID.ResearchComplete with { Volume = 0.35f, Pitch = -0.3f }, Owner.Center);

                    int totalDusts = 5;
                    float starAngle = MathHelper.Pi / totalDusts;
                    for (int i = 0; i < totalDusts; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB);
                        Vector2 vel = (MathHelper.TwoPi * i / totalDusts).ToRotationVector2().RotatedBy(starAngle) * 2f;
                        Dust dust2 = Dust.NewDustPerfect(GunTipPosition, DustID.FireworksRGB, vel, 80, Color.SandyBrown, 1.2f);
                        dust2.noGravity = true;
                    }

                    // reset variables
                    mogPlayer.las13Heat = 0;
                    Overheating = RemindReload = false;
                    ReloadTimer = ReminderTimer = 0;
                }
            }
            // if the gun is not overheating, fire
            else if (!Overheating)
            {
                if (leftShootChecks) Shoot(shootVelocity);
                if (BuiltHeat < MaxHeat && Main.mouseLeft)
                {
                    if (BuiltHeat < MaxHeat) mogPlayer.las13Heat++;
                }
                else if (BuiltHeat > 0 && !Main.mouseLeft) mogPlayer.las13Heat -= 2;
            }
            else
            {
                ReminderTimer++;
                // remind the player to reload after some time
                if (ReminderTimer >= LAS13Trident.reloadTime && !RemindReload)
                {
                    var Hotkey = KeybindSystem.FirstWeaponKeybind.TooltipHotkeyString();
                    Rectangle r = new((int)Owner.Hitbox.X, (int)Owner.Hitbox.Y - 20, Owner.Hitbox.Width, Owner.Hitbox.Height);
                    CombatText.NewText(r, Color.IndianRed, MiscUtils.GetTextFromModItem<LAS13Trident>("ReloadReminder").Format(Hotkey).ToString(), true);

                    SoundEngine.PlaySound(SoundID.Chat with { PitchVariance = 0.3f, MaxInstances = -1 }, Owner.Center);
                    RemindReload = true;
                }
                // Draw smoke effect while overheated
                if (Main.rand.NextBool(3))
                {
                    Dust smoke = Dust.NewDustPerfect(GunTipPosition, DustID.Smoke, new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-2f, -5f)) * 5.5f, newColor: Color.WhiteSmoke, Scale: 1.55f);
                    smoke.noGravity = true;
                    smoke.fadeIn = 0.4f;
                    smoke.scale *= 0.98f;
                    smoke.color = Color.DarkGray;
                    if (Main.rand.NextBool(4)) smoke.scale *= 1.2f;
                }
            } 
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
                    Dust heatDust = Dust.NewDustPerfect(Projectile.Center, DustID.YellowTorch, dustVel, Scale: 1.5f);
                    heatDust.noGravity = true;
                }

                Overheating = true;
                return;
            }
            if (BuiltHeat >= WarningTime && !playedWarningSound) WarningEffect();
            if (BuiltHeat < WarningTime) playedWarningSound = false;

            SoundEngine.PlaySound(SoundID.Item91 with { Volume = 0.8f, Pitch = Main.rand.NextFloat(-0.25f, -0.1f) }, Owner.Center);
            Owner.PickAmmo(Owner.HeldItem, out int ammo, out float speed, out int bulletDamage, out float knockback, out _, true);
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 shootPos = Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.25f;
                var source = Projectile.GetSource_FromThis();
                int type = ModContent.ProjectileType<LAS13Proj>();
                float value = MathHelper.Lerp(MinHeatDamage, MaxHeatDamage, BuiltHeat * 0.01f);
                int damage = (int)(bulletDamage * value);
                for (int index = 0; index < LAS13Trident.NumBeams; ++index)
                {
                    Projectile.NewProjectile(source, GunTipPosition, shootVelocity.RotatedByRandom(MathHelper.ToRadians(MathHelper.Lerp(1.4f, 4f, BuiltHeat * 0.01f))), type, damage, knockback, Projectile.owner, 0f, 0f);
                }

                ShootTimer = (int)(attackTime / ShootSpeed);
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
            Color tintColor = Overheating ? Color.Black : BuiltHeat >= WarningTime ? Color.Lerp(Color.Goldenrod, Color.White, MathF.Abs(MathF.Sin(Owner.miscCounter * MathHelper.Pi / 30f))) : Color.White;
            float opacity = Utils.GetLerpValue(0, WarningTime, BuiltHeat, true);

            for (int i = 0; i < 16; i++)
            {
                Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/LAS13Ghost").Value;
                Color auraColor = Color.Goldenrod * opacity * 0.6f;
                Vector2 drawOffset = ((MathHelper.TwoPi * i / 16f).ToRotationVector2() * 2);
                Main.EntitySpriteDraw(ghost, drawPosition + drawOffset, null, auraColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            }
            Main.EntitySpriteDraw(texture, drawPosition, null, tintColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            return false;
        }
    }
}