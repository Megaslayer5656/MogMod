using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class WyvernJawbladeHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public Player Owner => Main.player[Projectile.owner];
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<WyvernJawblade>()).Item;
        public override string Texture => BaseItem.ModItem.Texture;
        public override int AfterImageLength => 10;
        public override int OffsetDistance => 50;
        public override int CooldownTime { get; set; }
        public override bool AlternateSwings => false;
        //public override bool UseAttackSpeed => false;
        public override SoundStyle? UseSound => SoundID.DD2_MonkStaffSwing with { Volume = 1f };
        public ref float CurrentChargeMult => ref Projectile.ai[0];
        bool hasSmashedTile = false;
        bool playedChargeSound = false;
        bool firstEnemyHit = true;
        public override void Defaults()
        {
            Projectile.extraUpdates = 3;
            swingWidth = 200;
            RotateInCooldown = 0;
            RotateInStartup = 0;
            Projectile.width = 80;
            Projectile.height = 88;
        }
        public override void Spawn()
        {
            angle = new Vector2(angle.X.DirectionalSign(), 0);
            var player = Main.player[Projectile.owner];
            var modplayer = player.GetModPlayer<BaseSwordHoldoutPlayer>();
            StartupTime = Main.zenithWorld ? 360 : 80;
            CooldownTime = 30;
            swingTime = 10;
            modplayer.swingNum = 0;
            Projectile.timeLeft = 600;
            Projectile.scale *= 1.25f;
        }
        public override void AdditionalAI()
        {
            if (inStartup)
            {
                CurrentChargeMult = timer / (float)(StartupTime - 1);
                Owner.velocity.X *= 0.97f;
            }
            if (inStartup && !Owner.channel && timer > 30)
            {
                timer = StartupTime - 1;
            }
            if (Owner.channel && timer == StartupTime - 1)
            {
                Projectile.timeLeft++;
                timer--;
                if (!playedChargeSound)
                {
                    SoundEngine.PlaySound(SoundID.DeerclopsStep with { Volume = 2f, Pitch = 0.5f }, Projectile.Center);
                    playedChargeSound = true;
                    for (int i = 0; i < 5; i++)
                    {
                        float scale = Main.rand.NextFloat(0.5f, 1f);
                        var color = Main.rand.NextBool() ? Color.LightGoldenrodYellow : Color.Red;

                        if (Main.rand.NextBool(5))
                            scale *= 1.4f;
                        Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * MathHelper.Lerp(10, 30, Main.rand.NextFloat());
                        Dust d = Dust.NewDustPerfect(Projectile.Center + angle * 30, DustID.AncientLight, velocity, 100, color, scale);
                    }
                }
            }
            if (!hasSmashedTile && inSwing && SwingCompletion > 0.275f)
            {
                var adjustedAngle = angle.RotatedBy(Projectile.spriteDirection * SwingFunction());
                Vector2 HammerFrontPos = Projectile.Center + adjustedAngle * -16 * Projectile.scale + (adjustedAngle.RotatedBy(MathHelper.PiOver2) * 20 * Projectile.scale * angle.X);
                if (Collision.SolidCollision(HammerFrontPos, 1, 1))
                {
                    Owner.velocity *= 0.15f;
                    Owner.velocity -= adjustedAngle.RotatedBy(MathHelper.PiOver2) * angle.X * MathHelper.Lerp(2f, Main.zenithWorld ? 100f : 10f, CurrentChargeMult);
                    float ringRot = SwingCompletion < 0.5f ? 0 : MathHelper.PiOver2;
                    int radius = (int)(4 * CurrentChargeMult);
                    Point scanAreaStart = HammerFrontPos.ToTileCoordinates() + new Point(-radius, -radius);
                    Point scanAreaEnd = HammerFrontPos.ToTileCoordinates() + new Point(radius, radius);
                    Projectile.CreateImpactExplosion((int)(10 * CurrentChargeMult), Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out bool causedShockwaves);

                    hasSmashedTile = true;
                    timer = StartupTime + swingTime;
                    angle = adjustedAngle;
                    var pos = Projectile.Center;
                    Projectile.Size *= 2 + CurrentChargeMult;
                    Projectile.Center = HammerFrontPos;
                    Projectile.Damage();
                    Projectile.Size /= 2 + CurrentChargeMult;
                    Projectile.Center = pos;

                    if (CurrentChargeMult >= 1)
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { VariantsWeights = new ReadOnlySpan<float>(new float[] { 1, 0, 0 }) });
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact);
                }
            }
            Owner.heldProj = Projectile.whoAmI;
        }
        public override float SwingFunction()
        {
            if (hasSmashedTile) return MathHelper.ToRadians(MathHelper.Lerp(0, -swingWidth * 0.4f, MathF.Pow(CooldownCompletion, 0.5f)));
            if (inStartup) return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.8f, -swingWidth * 0.66f, 1 - MathF.Pow(StartupCompletion, 0.5f)));
            if (inCooldown) return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * 0.33f, swingWidth * 0.45f, MathF.Pow(CooldownCompletion, 0.5f)));
            return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * .66f, (swingWidth * 0.33f), SwingCompletion));
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.SourceDamage *= CurrentChargeMult * 4.8f;
            modifiers.Knockback += (CurrentChargeMult);
            //Main.NewText($"charge mult = {CurrentChargeMult}");
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hasSmashedTile && CurrentChargeMult >= 1)
            {
                for (int i = 0; i < 16; i++)
                {
                    int sparkLifetime = Main.rand.Next(10, 15);
                    float sparkScale = Main.rand.NextFloat(1f, 2f);
                    var sparkColor = Main.rand.NextBool() ? Color.Purple : Color.Red;

                    if (Main.rand.NextBool(5))
                        sparkScale *= 1.4f;

                }
                SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack with { Volume = 0.5f, LimitsArePerVariant = false, MaxInstances = 1 });
            }
            else if (!hasSmashedTile)
            {
                SoundEngine.PlaySound(SoundID.Item69 with { Volume = 1f, LimitsArePerVariant = false, MaxInstances = 1 });
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!inCooldown)
            {
                var tex = ModContent.Request<Texture2D>(Texture).Value;
                float outlineWidth = (int)(4 * CurrentChargeMult) * 0.5f;
                if (inSwing)
                {
                    outlineWidth *= 1 - SwingCompletion;
                }
                for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.25f)
                {
                    Main.spriteBatch.Draw(
                        tex,
                        Projectile.Center + new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX.RotatedBy(i + Projectile.rotation) * outlineWidth * Projectile.scale - Main.screenPosition,
                        null,
                        Color.Lerp(Color.LightGoldenrodYellow, Color.Red, CurrentChargeMult),
                        Projectile.rotation,
                        tex.Size() * 0.5f,
                        Projectile.scale,
                        Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                        0
                    );
                }
            }
            if (inSwing) return base.PreDraw(ref lightColor);
            return true;
        }
    }
}