using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
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
    public class BladeOfSelvesHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override int swingWidth => 210;
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<BladeOfSelves>()).Item;
        public override LocalizedText DisplayName => MiscUtils.GetItemName<BladeOfSelves>();
        public override string Texture => ModContent.GetModItem(BaseItem.type).Texture;
        public override int OffsetDistance => 36;
        public override int StartupTime { get; set; }
        public override int CooldownTime { get; set; }
        public override float lineCollisionLength => 32;
        public Player Owner => Main.player[Projectile.owner];
        public override SoundStyle? UseSound => SoundID.Item15 with { Volume = 0.9f, Pitch = Main.rand.NextFloat(0.1f, 0f) };
        bool playedSwingSound = false;
        Color Color1 = BladeOfSelves.MainColor1;
        Color Color2 = BladeOfSelves.MainColor2;
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
            Projectile.scale *= 1.75f;
            RotateInStartup = 1;
            Owner.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum = 1;
        }
        public override void AdditionalAI()
        {
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            if (inSwing)
            {
                if (!playedSwingSound)
                {
                    SoundEngine.PlaySound(SoundID.DD2_SonicBoomBladeSlash with { Volume = 0.8f, Pitch = Main.rand.NextFloat(0.35f, 0.55f) }, Projectile.Center);
                    playedSwingSound = true;
                }
                var veloc = oldPlayerOffset - (Projectile.Center - Main.player[Projectile.owner].Center);
                veloc.Normalize();
                float maxRotationDeviance = 0.8f;
                float rotationAngle = Main.rand.NextFloat(-maxRotationDeviance, maxRotationDeviance);
                float scale = Main.rand.NextFloat(0.7f, 1.15f);
                Vector2 velocity = veloc.RotatedBy(MathHelper.PiOver4 * 0.5f * Projectile.spriteDirection) * Main.rand.NextFloat(1, 4);

                for (int i = 0; i < 1; i++)
                {
                    Dust outerDust = Dust.NewDustPerfect(Projectile.Center + new Vector2(-angle.X.DirectionalSign(), Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation - 0.7f * Projectile.spriteDirection) * (Main.rand.NextFloat(-30, -45) * (mogPlayer.swingNum % 2 == 0 ? -1f : 1f)) * Projectile.scale, DustID.FireworksRGB, velocity, 100, Color.WhiteSmoke, scale);
                    outerDust.scale *= Main.rand.NextFloat(0.75f, 1.05f);
                    if (Main.rand.NextBool(4)) outerDust.scale *= Main.rand.NextFloat(0.25f, 1.65f);
                    outerDust.noGravity = true;
                    outerDust.color = Color.Lerp(Color2, Color1, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f);
                }
            }
            if (inCooldown && CooldownCompletion >= 1f && mogPlayer.swingNum % 2 != 0)
            {
                mogPlayer.swingNum++;
                timer = swingTimer = 0;
                Projectile.numHits = 0;
                Projectile.ResetLocalNPCHitImmunity();
            }
        }
        public override float SwingFunction()
        {
            if (inStartup) return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * -0.2f, swingWidth * -0.6f, StartupCompletion));
            if (inCooldown) return MathHelper.ToRadians(MathHelper.Lerp(swingWidth * 0.2f, swingWidth * 0.33f, CooldownCompletion));
            return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * -0.6f, swingWidth * 0.2f, SwingCompletion));
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (Owner.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum % 2 == 0)
            {
                modifiers.SourceDamage *= 1.5f;
                modifiers.Knockback += 1f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if ((target.life <= 0 && target.realLife == -1) && Projectile.numHits <= 2) Projectile.numHits -= 1;
            if (Projectile.numHits <= 2)
            {
                Vector2 launchVel = Utils.DirectionTo(Owner.Center, Owner.MogMod().mouseWorld);
                if (Projectile.numHits == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { Volume = 0.35f, PitchVariance = 0.15f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_BookStaffCast with { Volume = 0.4f, PitchVariance = 0.15f }, Projectile.Center);

                    int dustNum = (int)MathHelper.Clamp(12 - Projectile.numHits * 3, 3, 12);
                    for (int i = 0; i < dustNum; i++)
                    {
                        float variance = Main.rand.NextFloat(-0.5f, 0.5f);
                        int dustStyle = 278;
                        Dust dust2 = Dust.NewDustPerfect(target.Center, dustStyle, Projectile.velocity);
                        dust2.scale = Main.rand.NextFloat(1.2f, 1.4f) - Math.Abs(variance);
                        dust2.velocity = (launchVel * 25).RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance));
                        dust2.noGravity = true;
                        dust2.color = Main.rand.NextBool() ? Color1 : Color2;
                    }
                }
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss with { Volume = 0.65f, Pitch = 0.8f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss with { Volume = 0.55f, Pitch = 0.4f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item9 with { Volume = 0.45f, Pitch = 0.4f }, Projectile.Center);
                var source = Projectile.GetSource_FromThis();
                var type = ModContent.ProjectileType<BladeOfSelvesProj>();
                bool flip = Owner.direction == 1;
                if (Projectile.numHits <= 1)
                {
                    if (Projectile.owner == Main.myPlayer && Owner.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum % 2 == 0)
                        MogModUtils.ProjectileBarrage(source, Owner.Center, target.Center, flip, 150f, 150f, -50f, 40f, 10f, type, Projectile.damage, 0f, Projectile.owner, false, 0f, ai2: 0f);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/BladeOfSelvesGhost").Value;
            float outlineWidth = 4;
            if (!inCooldown) outlineWidth *= 1 - SwingCompletion;
            if (inSwing || inCooldown)
            {
                Texture2D swoosh = ModContent.Request<Texture2D>("MogMod/Assets/Textures/VerticalSmearLarge").Value;
                float rotation = Projectile.rotation - 0.7f * -Projectile.spriteDirection;
                float rotationOffset = (Owner.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum % 2 == 0 ? MathHelper.PiOver4 : (MathHelper.TwoPi - MathHelper.PiOver4)) * (angle.X < 0 ? -1f : 1f);
                Vector2 spawnPos = Projectile.Center + new Vector2(-angle.X.DirectionalSign(), 42f).RotatedBy(rotation) * Projectile.scale - Main.screenPosition;
                float fadeIn = Math.Min(1f, Math.Clamp(1f - (CooldownCompletion + 0.5f), 0f, 1f));
                Main.EntitySpriteDraw(swoosh, spawnPos, null, Color1 with { A = 0 } * (SwingCompletion * 0.75f) * (SwingCompletion >= 0.65f ? fadeIn : 1f), rotation + rotationOffset, swoosh.Size() * 0.5f, Projectile.scale * 0.25f, SpriteEffects.None);
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