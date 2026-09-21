using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Systems;
using MogMod.Items.Ammo.SorcerySpells.Carian;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System.Linq;
using MogMod.Common.Classes;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class CarianGreatswordHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public override int swingWidth => 270;
        public override bool UsesBaseItem => false;
        public override LocalizedText DisplayName => MiscUtils.GetItemName<CarianGreatsword>();
        public override string Texture => "MogMod/Projectiles/MagicProjectiles/Sorceries/CarianGreatswordHoldout";
        public override int OffsetDistance => 80;
        public override int StartupTime { get; set; }
        public override int CooldownTime { get; set; }
        public override float lineCollisionLength => 32;
        public Player Owner => Main.player[Projectile.owner];
        public override SoundStyle? UseSound => SoundID.Item9 with { Volume = 0.9f, Pitch = Main.rand.NextFloat(-0.2f, -0.1f) };
        public ref float HitsLeft => ref Projectile.ai[0];
        bool playedSwingSound = false;
        public Vector2 aimVel;
        Color Color1 = Color.DeepSkyBlue;
        Color Color2 = Color.SkyBlue;
        public override void Defaults()
        {
            Projectile.width = Projectile.height = 130;
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.extraUpdates = 5;
        }
        public override void Spawn()
        {
            Projectile.numHits = 0;
            StartupTime = 20;
            CooldownTime = 10;
            swingTime = 8;
            Projectile.scale *= 1.25f;
            RotateInStartup = 1;
            HitsLeft = CarianGreatsword.MaxReflects;
        }
        public override void AdditionalAI()
        {
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            if (inSwing)
            {
                if (!playedSwingSound)
                {
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.9f, Pitch = Main.rand.NextFloat(0.1f, 0f) }, Projectile.Center);
                    playedSwingSound = true;
                }
                if (Projectile.owner == Main.myPlayer) Reflect(Projectile);
                var veloc = oldPlayerOffset - (Projectile.Center - Main.player[Projectile.owner].Center);
                veloc.Normalize();
                float maxRotationDeviance = 0.8f;
                float rotationAngle = Main.rand.NextFloat(-maxRotationDeviance, maxRotationDeviance);
                float scale = Main.rand.NextFloat(0.7f, 1.15f);
                Vector2 velocity = veloc.RotatedBy(MathHelper.PiOver4 * 0.5f * Projectile.spriteDirection) * Main.rand.NextFloat(1, 4);

                for (int i = 0; i < 1; i++)
                {
                    Dust outerDust = Dust.NewDustPerfect(Projectile.Center + new Vector2(-angle.X.DirectionalSign(), Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation - 0.7f * Projectile.spriteDirection) * (Main.rand.NextFloat(-(Projectile.width / 2), -(Projectile.width - (Projectile.width / 4))) * (mogPlayer.swingNum % 2 == 0 ? -1f : 1f)) * Projectile.scale, DustID.FireworksRGB, velocity, 100, Color.WhiteSmoke, scale);
                    outerDust.scale *= Main.rand.NextFloat(0.75f, 1.05f);
                    if (Main.rand.NextBool(4)) outerDust.scale *= Main.rand.NextFloat(0.25f, 1.65f);
                    outerDust.noGravity = true;
                    outerDust.color = Color.Lerp(Color2, Color1, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f);
                }
            }
            if (inCooldown && CooldownCompletion >= 1f && Owner.channel)
            {
                mogPlayer.swingNum++;
                Projectile.timeLeft = ExistsTime * 2;
                timer = swingTimer = 0;
                Projectile.numHits = 0;
                HitsLeft = CarianGreatsword.MaxReflects;
                playedSwingSound = false;
                Projectile.ResetLocalNPCHitImmunity();
            }
        }
        public override float SwingFunction()
        {
            if (inStartup) return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * -0.33f, swingWidth * -0.6f, StartupCompletion));
            if (inCooldown) return MathHelper.ToRadians(MathHelper.Lerp(swingWidth * 0.2f, swingWidth * 0.33f, CooldownCompletion));
            return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * -0.6f, swingWidth * 0.2f, SwingCompletion));
        }
        // copied from fargos hallow sword
        private void Reflect(Projectile sword)
        {
            if (Owner == null || !Owner.active) return;
            int damageCap = 200;

            float size = 2.5f;
            Rectangle swordBox = new((int)(sword.Center.X - sword.width * size / 2), (int)(sword.Center.Y - sword.height * size / 2), (int)(sword.Hitbox.Width * size), (int)(sword.Hitbox.Height * size));
            foreach (Projectile proj in Main.projectile.Where(proj => proj.active && proj.hostile && proj.damage > 0 && proj.type != null && proj.damage <= damageCap && sword.Colliding(swordBox, proj.Hitbox)))
            {
                if (HitsLeft <= 0 || MogModProjectileSets.ShouldNotBeReflected[proj.type]) return;
                aimVel = (proj.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * 65f;
                SoundEngine.PlaySound(SoundID.Item37, proj.Center);
                proj.reflected = true;
                proj.hostile = false;
                proj.friendly = true;
                proj.owner = sword.owner;
                proj.damage = sword.damage;
                proj.DamageType = sword.DamageType;
                Vector2 targetVel = -(aimVel / 10);
                proj.velocity = targetVel;
                proj.netUpdate = true;
                HitsLeft--;

                int dustNum = 9;
                for (int i = 0; i < dustNum; i++)
                {
                    float variance = Main.rand.NextFloat(-0.5f, 0.5f);
                    int dustStyle = 278;
                    Dust dust2 = Dust.NewDustPerfect(proj.Center, dustStyle, Projectile.velocity);
                    dust2.scale = Main.rand.NextFloat(1.2f, 1.4f) - Math.Abs(variance);
                    dust2.velocity = (targetVel * 3f).RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance));
                    dust2.noGravity = true;
                    dust2.color = Main.rand.NextBool() ? Color1 : Color2;
                }
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
                    SoundEngine.PlaySound(SoundID.Item8 with { Volume = 0.4f, PitchVariance = 0.15f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot with { Volume = 0.5f, Pitch = 0.6f, PitchVariance = 0.15f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss with { Volume = 0.65f, Pitch = 0.8f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss with { Volume = 0.55f, Pitch = 0.4f }, Projectile.Center);

                    int dustNum = (int)MathHelper.Clamp(15 - Projectile.numHits * 5, 5, 15);
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
                SoundEngine.PlaySound(SoundID.Item69 with { Volume = 0.35f, Pitch = 1f, LimitsArePerVariant = false, MaxInstances = 1 });
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/CarianGreatswordGhost").Value;
            float outlineWidth = 4;
            if (!inCooldown) outlineWidth *= 1 - SwingCompletion;
            if (inSwing || inCooldown)
            {
                Texture2D swoosh = ModContent.Request<Texture2D>("MogMod/Assets/Textures/VerticalSmearLarge").Value;
                float rotation = Projectile.rotation - 0.7f * -Projectile.spriteDirection;
                float rotationOffset = (Owner.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum % 2 == 0 ? MathHelper.PiOver4 : (MathHelper.TwoPi - MathHelper.PiOver4)) * (angle.X < 0 ? -1f : 1f);
                Vector2 spawnPos = Projectile.Center + new Vector2(-angle.X.DirectionalSign(), 60f).RotatedBy(rotation) * Projectile.scale - Main.screenPosition;
                float fadeIn = Math.Min(1f, Math.Clamp(1f - (CooldownCompletion + 0.5f), 0f, 1f));
                Main.EntitySpriteDraw(swoosh, spawnPos, null, Color1 with { A = 0 } * (SwingCompletion * 0.75f) * (SwingCompletion >= 0.65f ? fadeIn : 1f), rotation + rotationOffset, swoosh.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None);
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