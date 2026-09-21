using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Classless;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class GreatswordOfSoulsHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override int swingWidth => 240;
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<GreatswordOfSouls>()).Item;
        public override LocalizedText DisplayName => MiscUtils.GetItemName<GreatswordOfSouls>();
        public override string Texture => ModContent.GetModItem(BaseItem.type).Texture;
        public override int OffsetDistance => 66;
        public override int StartupTime { get; set; }
        public override int CooldownTime { get; set; }
        public override float lineCollisionLength => 32;
        public Player Owner => Main.player[Projectile.owner];
        public override SoundStyle? UseSound => SoundID.DD2_MonkStaffSwing with { Volume = 0.9f, Pitch = Main.rand.NextFloat(0.1f, 0f) };
        Color Color1 = GreatswordOfSouls.MainColor1;
        Color Color2 = GreatswordOfSouls.MainColor2;
        float soulDamage = 0.75f;
        public override void Defaults()
        {
            Projectile.extraUpdates = 5;
        }
        public override void Spawn()
        {
            Projectile.numHits = 0;
            StartupTime = 12;
            CooldownTime = 12;
            swingTime = 8;
            Projectile.scale *= 1.5f;
            RotateInStartup = 1;
        }
        public override void AdditionalAI()
        {
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            if (inSwing)
            {
                var veloc = oldPlayerOffset - (Projectile.Center - Main.player[Projectile.owner].Center);
                veloc.Normalize();
                float maxRotationDeviance = 0.8f;
                float rotationAngle = Main.rand.NextFloat(-maxRotationDeviance, maxRotationDeviance);
                float scale = Main.rand.NextFloat(0.7f, 1.15f);
                Vector2 velocity = veloc.RotatedBy(MathHelper.PiOver4 * 0.5f * Projectile.spriteDirection) * Main.rand.NextFloat(1, 4);

                Vector2 dustVel = new Vector2(-10 * Projectile.spriteDirection, -5).RotatedBy(Projectile.rotation);
                for (int i = 0; i < 1; i++)
                {
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + dustVel.RotatedByRandom(0.4f) * Projectile.scale, DustID.FireworksRGB, velocity, 100, Color.WhiteSmoke, scale);
                    dust2.velocity *= 1.05f;
                    if (Main.rand.NextBool(4)) dust2.velocity *= 1.85f;
                    dust2.scale *= Main.rand.NextFloat(0.75f, 1.05f);
                    if (Main.rand.NextBool(4)) dust2.scale *= Main.rand.NextFloat(0.25f, 1.65f);
                    dust2.noGravity = true;
                    if (Main.rand.NextBool(2)) dust2.noGravity = false;
                    dust2.color = Color.Lerp(Color1, Color2, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f);
                }

                for (int i = 0; i < 1; i++)
                {
                    Dust outerDust = Dust.NewDustPerfect(Projectile.Center + new Vector2(-angle.X.DirectionalSign(), Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation - 0.7f * Projectile.spriteDirection) * (Main.rand.NextFloat(-30, -45) * (mogPlayer.swingNum % 2 == 0 ? -1f : 1f)) * Projectile.scale, DustID.FireworksRGB, velocity, 100, Color.WhiteSmoke, scale);
                    outerDust.scale *= Main.rand.NextFloat(0.75f, 1.05f);
                    if (Main.rand.NextBool(4)) outerDust.scale *= Main.rand.NextFloat(0.25f, 1.65f);
                    outerDust.noGravity = true;
                    outerDust.color = Color.Lerp(Color2, Color1, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f);
                }
            }
        }
        public override float SwingFunction()
        {
            if (inStartup) return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * -0.2f, swingWidth * -0.6f, StartupCompletion));
            if (inCooldown) return MathHelper.ToRadians(MathHelper.Lerp(swingWidth * 0.2f, swingWidth * 0.33f, CooldownCompletion));
            return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * -0.6f, swingWidth * 0.2f, SwingCompletion));
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if ((target.life <= 0 && target.realLife == -1) && Projectile.numHits <= 2) Projectile.numHits -= 1;
            if (Projectile.numHits < 3)
            {
                if (soulDamage > 0f)
                {
                    SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost with { Volume = 0.35f, PitchVariance = 0.15f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { Volume = 0.4f, PitchVariance = 0.15f }, Projectile.Center);
                    //SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact with { Volume = 0.75f, PitchVariance = 0.15f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot with { Volume = 0.45f });
                    SoundEngine.PlaySound(SoundID.Item69 with { Volume = 0.35f, LimitsArePerVariant = false, MaxInstances = 1 });

                    float starAngle = MathHelper.ToRadians(45f);
                    var type = ModContent.ProjectileType<GreatswordOfSoulsProj>();
                    int Damage(float mult = 1f) => (int)(Projectile.damage * mult);
                    for (int i = 0; i < 4; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, newColor: Main.rand.NextBool() ? Color1 : Color2);
                        Vector2 vel(float mult = 1f) => (MathHelper.TwoPi * i / 4f).ToRotationVector2().RotatedBy(starAngle) * mult;
                        Dust dust2 = Dust.NewDustPerfect(target.Center, DustID.FireworksRGB, vel(4f), 80, Color2, 1.2f * Projectile.scale);
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Projectile soul = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, vel(12f), type, Damage(soulDamage), Projectile.knockBack, Projectile.owner);
                            soul.DamageType = Projectile.DamageType;
                            soul.ai[2] = 25f;
                        }
                    }
                    soulDamage -= 0.25f;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/SoulGreatswordGhost").Value;
            float outlineWidth = 4;
            if (!inCooldown) outlineWidth *= 1 - SwingCompletion;
            if (inSwing || inCooldown)
            {
                Texture2D swoosh = ModContent.Request<Texture2D>("MogMod/Assets/Textures/VerticalSmearLarge").Value;
                float rotation = Projectile.rotation - 0.7f * -Projectile.spriteDirection;
                float rotationOffset = (Owner.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum % 2 == 0 ? MathHelper.PiOver4 : (MathHelper.TwoPi - MathHelper.PiOver4)) * (angle.X < 0 ? -1f : 1f);
                Vector2 spawnPos = Projectile.Center + new Vector2(-angle.X.DirectionalSign(), 52f).RotatedBy(rotation) * Projectile.scale - Main.screenPosition;
                float fadeIn = Math.Min(1f, Math.Clamp(1f - (CooldownCompletion + 0.5f), 0f, 1f));
                Main.EntitySpriteDraw(swoosh, spawnPos, null, Color1 with { A = 0 } * (SwingCompletion * 0.75f) * (SwingCompletion >= 0.65f ? fadeIn : 1f), rotation + rotationOffset, swoosh.Size() * 0.5f, Projectile.scale * 0.35f, SpriteEffects.None);
                for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.25f)
                {
                    Main.spriteBatch.Draw(ghost,
                        Projectile.Center + new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX.RotatedBy(i + Projectile.rotation) * outlineWidth * Projectile.scale - Main.screenPosition,
                        null,
                        Color.Lerp(Color1, Color2, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f),
                        Projectile.rotation,
                        ghost.Size() * 0.5f,
                        Projectile.scale,
                        Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                        0);
                }
            }
            return base.PreDraw(ref lightColor);
        }
    }
}