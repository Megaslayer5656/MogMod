using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Config;
using MogMod.Items.Weapons.Ranged;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class LAS13Holdout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<LAS13Trident>();
        public override Vector2 GunTipPosition => Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.5f;
        public override float MaxOffsetLengthFromArm => 18f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -5f;
        public override float OffsetYDownwards => 5f;
        public ref float ShootTimer => ref Projectile.ai[0];
        private int BuiltHeat => (Owner.HeldItem.ModItem as LAS13Trident).BuiltUpHeat;
        private const int WarningTime = LAS13Trident.OverheatLevel - 70;
        public int MaxHeat = LAS13Trident.OverheatLevel;
        public bool Overheating = false;
        public static readonly SoundStyle WarningSound = new($"{nameof(MogMod)}/Sounds/SE/ArmletOn") { Volume = 1.1f, PitchVariance = .2f, MaxInstances = 0 };
        public override void KillHoldoutLogic()
        {
            if (Owner.CantUseHoldout(false) || HeldItem.type != Owner.HeldItem.type || (BuiltHeat == 0 && !Main.mouseLeft))
            {
                Projectile.Kill();
            }
        }
        public override void HoldoutAI()
        {
            var attackSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
            float cap = 5f;
            if (attackSpeed > cap) attackSpeed = cap;
            if (attackSpeed != 0f) attackSpeed = 1f / attackSpeed;
            int cooldown = (int)(60 * attackSpeed);
            int attackCooldown = cooldown / 4;
            int newAttackCooldown = attackCooldown / 2;

            if (ShootTimer > attackCooldown)
            {
                (Owner.HeldItem.ModItem as LAS13Trident).BuiltUpHeat++;

                // Overheat yourself if you fire too long
                if (BuiltHeat >= MaxHeat)
                {
                    for (int e = 0; e < 7; e++)
                    {
                        Vector2 dustVel = -Projectile.rotation.ToRotationVector2().RotatedByRandom(MathHelper.Pi * 0.15f) * Main.rand.NextFloat(3.8f, 5.5f);
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.YellowTorch, dustVel, Scale: 1.5f);
                        dust.noGravity = true;
                    }

                    (Owner.HeldItem.ModItem as LAS13Trident).BuiltUpHeat = 1;
                    Owner.MogMod().lasOverheat = LAS13Trident.OverheatCooldown;
                    Overheating = true;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        if (MogClientConfig.Instance.AmmoEjection && Main.netMode != NetmodeID.Server)
                        {
                            string goreType = "HellfireMag"; // TODO: change this
                            Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedBy(2f * -Owner.direction) * Main.rand.NextFloat(0.6f, 0.7f), Mod.Find<ModGore>(goreType).Type);
                        }
                    }
                    return;
                }
                if (BuiltHeat == WarningTime) SoundEngine.PlaySound(WarningSound, Owner.Center);

                if (ShootTimer % (BuiltHeat >= WarningTime ? newAttackCooldown : attackCooldown) == 0 && BuiltHeat <= MaxHeat)
                {
                    ShootTimer++; // here so we dont rapidly fire every frame
                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 4;
                    SoundEngine.PlaySound(SoundID.Item91 with { Volume = 0.8f, Pitch = Main.rand.NextFloat(-0.25f, -0.1f) }, Projectile.Center);
                    Owner.PickAmmo(Owner.HeldItem, out _, out float shootSpeed, out int damage, out float knockback, out _);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        int bulletAmt = 6;
                        for (int index = 0; index < bulletAmt; ++index)
                        {
                            int type = ModContent.ProjectileType<LAS13Proj>();
                            var source = Projectile.GetSource_FromThis();
                            Projectile.NewProjectile(source, GunTipPosition, shootVelocity.RotatedByRandom(MathHelper.ToRadians(MathHelper.Lerp(1.4f, 4f, (Owner.HeldItem.ModItem as LAS13Trident).BuiltUpHeat * 0.01f))), type, damage, knockback, Projectile.owner, 0f, 0f);
                        }
                    }
                }
            }
            Overheating = Owner.MogMod().lasOverheat != 0;
            // Draw smoke effect while overheated
            if (Overheating && Main.rand.NextBool(3))
            {
                Dust smoke = Dust.NewDustPerfect(GunTipPosition, DustID.Smoke, new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-2f, -5f)) * 7.5f, newColor: Color.WhiteSmoke, Scale: 1.55f);
                smoke.noGravity = true;
                smoke.fadeIn = 0.4f;
                smoke.scale *= 0.98f;
                smoke.color = Color.Lerp(Color.Goldenrod, Color.DarkGray, MathF.Abs(MathF.Sin(Owner.MogMod().hellfireOverheat * MathHelper.Pi / 30f)));
                if (Main.rand.NextBool(4)) smoke.scale *= 1.2f;
            }
            if (Owner.MogMod().lasOverheat == 0) if (Main.mouseLeft) ShootTimer++;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            SoundEngine.PlaySound(SoundID.Item73 with { Volume = 0.7f }, Projectile.Center);
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
                Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Projectiles/RangedProjectiles/LAS13Ghost").Value;
                Color auraColor = Color.Goldenrod * opacity * 0.6f;
                Vector2 drawOffset = ((MathHelper.TwoPi * i / 16f).ToRotationVector2() * 5);
                Main.EntitySpriteDraw(ghost, drawPosition + drawOffset, null, auraColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            }
            Main.EntitySpriteDraw(texture, drawPosition, null, tintColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            return false;
        }
    }
}