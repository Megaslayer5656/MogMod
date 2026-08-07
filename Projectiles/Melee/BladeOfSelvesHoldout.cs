using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class BladeOfSelvesHoldout : BaseCustomUseStyleProjectile, ILocalizedModType
    {
        public override int AssignedItemID => ModContent.ItemType<BladeOfSelves>();
        public override LocalizedText DisplayName => MiscUtils.GetItemName<BladeOfSelves>();
        public override string Texture => "MogMod/Items/Weapons/Melee/BladeOfSelves";
        public int size = 50 + 10;
        public override float HitboxOutset => size * 0.85f;
        public override Vector2 HitboxSize => new(size, size);
        public override Vector2 SpriteOrigin => new(0, size - 10);
        public override float HitboxRotationOffset => MathHelper.ToRadians(-45);
        public override float AdditionalScale => 1.2f;
        public Vector2 mousePos;
        public Vector2 aimVel;
        public bool doSwing = true;
        public bool postSwing = false;
        public float fadeIn = 0;
        public int useAnim;
        public int swingCount;
        public bool finalFlip = false;
        public bool playSwingSound = true;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void WhenSpawned()
        {
            //Projectile.knockBack = 0;
            Projectile.ai[1] = 1;

            mousePos = Owner.MogMod().mouseWorld;
            aimVel = (Owner.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
            useAnim = (int)(Owner.itemAnimationMax * 0.5f); // 2 swings

            if (mousePos.X < Owner.Center.X) Owner.direction = -1;
            else Owner.direction = 1;

            FlipAsSword = Owner.direction == -1;
        }
        public override void UseStyle()
        {
            AnimationProgress = Animation % useAnim;
            DrawUnconditionally = false;

            if (CanHit || postSwing)
                mousePos = Owner.Center - aimVel;
            else
            {
                mousePos = Owner.MogMod().mouseWorld;
            }

            if (CanHit && postSwing)
                fadeIn = MathHelper.Lerp(fadeIn, 1, 0.15f);
            else
                fadeIn = MathHelper.Lerp(fadeIn, 0, 0.25f);


            if (!doSwing)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                    Projectile.localNPCImmunity[i] = 0;

                playSwingSound = true;
                Projectile.numHits = 0;
                mousePos = Owner.MogMod().mouseWorld;
                aimVel = (Owner.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                CanHit = false;
                if (mousePos.X < Owner.Center.X) Owner.direction = -1;
                else Owner.direction = 1;
                FlipAsSword = Owner.direction == -1;
                if (swingCount > 1) swingCount = 0;

                doSwing = true;
                finalFlip = false;
            }
            else
            {
                if (!CanHit && !postSwing)
                {
                    if (mousePos.X < Owner.Center.X) Owner.direction = -1;
                    else Owner.direction = 1;
                }
                else
                {
                    if ((Owner.Center - aimVel).X < Owner.Center.X) Owner.direction = -1;
                    else Owner.direction = 1;
                }


                Projectile.rotation = Projectile.rotation.AngleLerp(Owner.AngleTo(mousePos) + MathHelper.ToRadians(45f), 0.1f);

                if (AnimationProgress < (useAnim / 1.6f))
                {
                    aimVel = (Owner.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                    CanHit = false;
                    postSwing = false;
                    if (AnimationProgress == 0)
                    {
                        Animation = 0;
                        doSwing = false;
                        Projectile.ai[1] = -Projectile.ai[1];
                    }
                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(120f * Projectile.ai[1] * Owner.direction * (1 + (Utils.GetLerpValue(useAnim * 0.35f, useAnim * 0.65f, Animation, true)) * 0.45f)), 0.2f);
                    FlipAsSword = (Owner.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX).X > 0;
                }
                else
                {
                    if (!finalFlip)
                    {
                        FlipAsSword = Owner.direction < 0;
                    }

                    float time = (AnimationProgress) - (useAnim / 3);
                    float timeMax = useAnim - (useAnim / 3);

                    if (time >= (int)(timeMax * 0.4f) && playSwingSound)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_SonicBoomBladeSlash with { Volume = 0.8f, Pitch = Main.rand.NextFloat(0.35f, 0.55f) }, Projectile.Center);
                        SoundEngine.PlaySound(SoundID.Item15 with { Volume = 0.4f, Pitch = Main.rand.NextFloat(0.35f, 0.55f) }, Projectile.Center);
                        swingCount++;
                        playSwingSound = false;
                    }
                    if (time > (int)(timeMax * 0.5f))
                    {
                        CanHit = true;
                    }
                    else
                        CanHit = false;

                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(MathHelper.Lerp(150f * Projectile.ai[1] * Owner.direction, 120f * -Projectile.ai[1] * Owner.direction, MiscUtils.ExpInOutEasing(time / timeMax, 1))), 0.2f * Owner.GetAttackSpeed<MeleeDamageClass>());

                    if (time >= timeMax)
                        doSwing = false;
                    if (time < (int)(timeMax * 0.7f))
                        postSwing = true;

                    if (CanHit)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Dust dust2 = Dust.NewDustPerfect(Owner.Center + (new Vector2(70 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f)), DustID.FireworksRGB, Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 2));
                            dust2.scale = Main.rand.NextFloat(0.55f, 0.85f);
                            dust2.noGravity = true;
                            dust2.color = Main.rand.NextBool() ? Color.Pink : Color.Goldenrod;
                        }
                    }
                }
            }
            ArmRotationOffset = MathHelper.ToRadians(-140f);
            ArmRotationOffsetBack = MathHelper.ToRadians(-140f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if ((target.life <= 0 && target.realLife == -1) && Projectile.numHits > 0)
                Projectile.numHits -= 1;

            Vector2 launchVel = Utils.DirectionTo(Owner.Center, Owner.MogMod().mouseWorld);
            target.MoveNPC(launchVel, 9, true, Owner);

            int dustNum = (int)MathHelper.Clamp(12 - Projectile.numHits * 3, 3, 12);
            for (int i = 0; i < dustNum; i++)
            {
                float variance = Main.rand.NextFloat(-0.5f, 0.5f);
                int dustStyle = 278;
                Dust dust2 = Dust.NewDustPerfect(target.Center, dustStyle, Projectile.velocity);
                dust2.scale = Main.rand.NextFloat(1.2f, 1.4f) - Math.Abs(variance);
                dust2.velocity = (launchVel * 25).RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance));
                dust2.noGravity = true;
                dust2.color = Main.rand.NextBool() ? Color.Pink : Color.Goldenrod;
            }
            var source = Projectile.GetSource_FromThis();
            if (Projectile.owner == Main.myPlayer && Projectile.numHits <= 1)
            {
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss with { Volume = 0.65f, Pitch = 0.8f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss with { Volume = 0.55f, Pitch = 0.4f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item9 with { Volume = 0.45f, Pitch = 0.4f }, Projectile.Center);
                if (swingCount > 1 && Projectile.owner == Main.myPlayer)
                    MogModUtils.ProjectileBarrage(target.GetSource_FromAI(), target.Center, target.Center, !FlipAsSword, 150f, 150f, -150f, 150f, 10f, ModContent.ProjectileType<BladeOfSelvesProj>(), Projectile.damage, 0f, Projectile.owner, false, 0f, ai2: 0f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // Only draw the projectile if the projectile's owner is currently using the item this projectile is attached to.
            if ((useAnim > 0 || DrawUnconditionally) && Owner.ItemAnimationActive)
            {
                Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);

                float r = FlipAsSword ? MathHelper.ToRadians(90) : 0f;

                for (int i = 0; i < 25; i++)
                {
                    Color auraColor = Color.Pink with { A = 0 } * 0.15f * fadeIn;
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 25f).ToRotationVector2() * 5 * fadeIn;
                    Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + drawOffset + new Vector2(0, Owner.gfxOffY), tex.Frame(1, FrameCount, 0, Frame), auraColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                }
                Asset<Texture2D> swoosh = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/VerticalSmearLarge");

                if (swingCount > 0 && !playSwingSound)
                    Main.EntitySpriteDraw(swoosh.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), null, Color.Pink with { A = 0 } * fadeIn * 0.4f, (FinalRotation + MathHelper.ToRadians(45)) + MathHelper.ToRadians(swingCount % 2 != 0 ? -55 : 55) * -Owner.direction, swoosh.Size() * 0.5f, Projectile.scale * 0.85f / 4, swingCount % 2 != 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

                Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), tex.Frame(1, FrameCount, 0, Frame), lightColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
            }
            return false;
        }
        public override void ResetStyle()
        {
        }
    }
}