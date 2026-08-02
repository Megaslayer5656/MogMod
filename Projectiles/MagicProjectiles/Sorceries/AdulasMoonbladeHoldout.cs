using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Classes;
using MogMod.Items.Ammo.SorcerySpells.Carian;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System.Linq;
using MogMod.Common.Systems;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class AdulasMoonbladeHoldout : BaseCustomUseStyleProjectile, ILocalizedModType
    {
        public override int AssignedItemID => ModContent.ItemType<AdulasMoonblade>();
        public override LocalizedText DisplayName => MiscUtils.GetItemName<AdulasMoonblade>();
        public override string Texture => "MogMod/Projectiles/MagicProjectiles/Sorceries/AdulasMoonbladeHoldout";
        public override bool ItemUsedAsAmmo => true;
        public int size = 130;
        public int hitsLeft = 5;
        public override float HitboxOutset => size * 0.85f;
        public override Vector2 HitboxSize => new Vector2(size, size);
        public override Vector2 SpriteOrigin => new(0, size);
        public override float HitboxRotationOffset => MathHelper.ToRadians(-45);
        public override float AdditionalScale => 0.2f;
        public Vector2 mousePos;
        public Vector2 aimVel;
        public bool doSwing = true;
        public bool postSwing = false;
        public float fadeIn = 0;
        public int useAnim;
        public int swingCount;
        public bool finalFlip = false;
        public bool playSwingSound = true;
        public bool swooshFade = false;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = SorceryDamageClass.Instance;
        }
        public override void WhenSpawned()
        {
            CanHit = false;
            //Projectile.knockBack = 0;
            Projectile.ai[1] = -1;
            mousePos = Owner.MogMod().mouseWorld;
            aimVel = (Owner.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
            useAnim = Owner.itemAnimationMax;

            if (mousePos.X < Owner.Center.X) Owner.direction = -1;
            else Owner.direction = 1;

            FlipAsSword = Owner.direction == -1;
        }
        public override void UseStyle()
        {
            AnimationProgress = Animation % useAnim;
            DrawUnconditionally = false;
            Owner.MogMod().mouseWorldListener = true;

            if (CanHit || postSwing)
                mousePos = Owner.Center - aimVel;
            else
            {
                mousePos = Owner.MogMod().mouseWorld;
            }

            if (CanHit && !swooshFade)
                fadeIn = MathHelper.Lerp(fadeIn, 1, 0.5f);
            else
                fadeIn = MathHelper.Lerp(fadeIn, 0, 0.35f);


            if (!doSwing)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                    Projectile.localNPCImmunity[i] = 0;

                hitsLeft = 5;
                Projectile.numHits = 0;
                mousePos = Owner.MogMod().mouseWorld;
                aimVel = (Owner.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                CanHit = false;
                if (mousePos.X < Owner.Center.X) Owner.direction = -1;
                else Owner.direction = 1;
                FlipAsSword = Owner.direction == -1 ? true : false;
                doSwing = true;
                swingCount++;
                finalFlip = false;
                playSwingSound = true;
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

                if (AnimationProgress < (useAnim / 1.5f))
                {
                    aimVel = (Owner.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                    CanHit = false;
                    postSwing = false;
                    if (AnimationProgress == 0)
                    {
                        doSwing = false;
                        Projectile.ai[1] = -Projectile.ai[1];
                    }
                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(120f * Projectile.ai[1] * Owner.direction * (1 + (Utils.GetLerpValue(useAnim * 0.7f, useAnim, Animation, true)) * 0.35f)), 0.2f);
                }
                else
                {
                    if (!finalFlip)
                    {
                        FlipAsSword = Owner.direction < 0 ? true : false;
                    }

                    float time = (AnimationProgress) - (useAnim / 3);
                    float timeMax = useAnim - (useAnim / 3);

                    if (time >= (int)(timeMax * 0.4f) && playSwingSound)
                    {
                        SoundEngine.PlaySound(SoundID.Item15 with { Pitch = Main.rand.NextFloat(-0.4f, -0.5f) }, Projectile.Center);
                        SoundEngine.PlaySound(SoundID.Item15 with { Volume = 0.85f, Pitch = Main.rand.NextFloat(0.1f, 0.2f) }, Projectile.Center);
                        //if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -(aimVel / 4), ModContent.ProjectileType<AdulasMoonbladeProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        playSwingSound = false;
                    }
                    if (time > (int)(timeMax * 0.2f))
                        Reflect(Projectile);
                    if (time > (int)(timeMax * 0.2f) && time < (int)(timeMax * 0.8f))
                        CanHit = true;
                    else
                        CanHit = false;
                    if (time > (int)(timeMax * 0.7f))
                        swooshFade = true;
                    else
                        swooshFade = false;

                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(MathHelper.Lerp(150f * Projectile.ai[1] * Owner.direction, 120f * -Projectile.ai[1] * Owner.direction, MiscUtils.ExpInOutEasing(time / timeMax, 1))),
                        0.2f);

                    if (time >= timeMax)
                        doSwing = false;
                    if (time < (int)(timeMax * 0.7f))
                        postSwing = true;

                    if (CanHit)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(Owner.Center + (new Vector2((int)(size * 1.4f) * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f)), 15, Vector2.One.RotatedByRandom(MathHelper.Pi) * 0.6f, 100, default, Main.rand.NextFloat(1.15f, 1.5f) * Projectile.scale);
                            dust.noGravity = true;
                            dust.color = Main.rand.NextBool() ? Color.AliceBlue : Color.LightBlue;
                            dust.fadeIn = Projectile.scale - 1;
                        }
                        float randRot = Main.rand.NextFloat(-30, -60);
                        Vector2 dustVel = (new Vector2(0, 8 * -Projectile.ai[1] * Owner.direction)).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));

                        Dust d = Dust.NewDustPerfect(Owner.Center + new Vector2((int)(size * 1.4f) * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f), 15, dustVel, 100, Main.rand.NextBool(4) ? Color.AliceBlue : Color.LightBlue, Main.rand.NextFloat(0.4f, 0.8f));
                        d.noGravity = true;
                    }
                }
            }

            ArmRotationOffset = MathHelper.ToRadians(-(size + 10f));
            ArmRotationOffsetBack = MathHelper.ToRadians(-(size + 10f));
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if ((damageDone <= 2 || (target.life <= 0 && target.realLife == -1)) && Projectile.numHits > 0)
                Projectile.numHits -= 1;

            Vector2 launchVel = Utils.DirectionTo(Owner.Center, Owner.MogMod().mouseWorld);
            MogModUtils.MoveNPC(target, launchVel, 12, true, Owner);

            int dustNum = (int)MathHelper.Clamp(12 - Projectile.numHits * 3, 3, 12);
            for (int i = 0; i < dustNum; i++)
            {
                float variance = Main.rand.NextFloat(-0.5f, 0.5f);
                int dustStyle = 278;
                Dust dust2 = Dust.NewDustPerfect(target.Center, dustStyle, Projectile.velocity);
                dust2.scale = Main.rand.NextFloat(1.2f, 1.4f) - Math.Abs(variance);
                dust2.velocity = (launchVel * 25).RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance));
                dust2.noGravity = true;
                dust2.color = Main.rand.NextBool() ? Color.AliceBlue : Color.LightBlue;
            }

            if (Projectile.numHits == 0)
            {
                SoundEngine.PlaySound(SoundID.Item94 with { Volume = 0.95f, Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, Projectile.Center);
            }
        }
        // copied from fargos hallow sword
        private void Reflect(Projectile sword)
        {
            Player player = Main.player[sword.owner];
            if (player == null || !player.active)
            {
                return;
            }
            int damageCap = 350;

            int size = 3;
            Rectangle swordBox = new((int)(sword.Center.X - sword.width * size / 2), (int)(sword.Center.Y - sword.height * size / 2), sword.Hitbox.Width * size, sword.Hitbox.Height * size);
            foreach (Projectile proj in Main.projectile.Where(proj => proj.active && proj.hostile && proj.damage > 0 && !MogModProjectileSets.ShouldNotBeReflected[proj.type] && proj.damage <= damageCap && sword.Colliding(swordBox, proj.Hitbox)))
            {
                if (hitsLeft <= 0)
                    return;
                SoundEngine.PlaySound(SoundID.Item37, proj.Center);
                proj.reflected = true;
                proj.hostile = false;
                proj.friendly = true;
                proj.owner = sword.owner;
                proj.damage = (int)(sword.damage * 2f);
                proj.DamageType = sword.DamageType;
                const int speed = 15;
                Vector2 targetVel = -(aimVel / 4);
                int target = proj.FindTargetWithLineOfSight(800);
                NPC targetNPC = Main.npc[target];
                if (targetNPC != null && targetNPC.active && !targetNPC.townNPC)
                    targetVel = Vector2.Normalize(targetNPC.Center - proj.Center) * speed;
                proj.velocity = targetVel;
                proj.netUpdate = true;
                hitsLeft--;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // Only draw the projectile if the projectile's owner is currently using the item this projectile is attached to.
            if ((useAnim > 0 || DrawUnconditionally) && Owner.ItemAnimationActive)
            {
                Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
                Asset<Texture2D> swoosh = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/VerticalSmearLarge");

                float r = FlipAsSword ? MathHelper.ToRadians(90) : 0f;

                // sick ass light around the sword
                for (int i = 0; i < 20; i++)
                {
                    Color auraColor = Color.LightBlue with { A = 0 } * 0.18f * fadeIn;
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 20f).ToRotationVector2() * 4 * fadeIn;
                    Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + drawOffset + new Vector2(0, Owner.gfxOffY), tex.Frame(1, FrameCount, 0, Frame), auraColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                }

                Main.EntitySpriteDraw(swoosh.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), null, Color.SkyBlue with { A = 0 } * fadeIn * 0.5f, (FinalRotation + MathHelper.ToRadians(45)) + MathHelper.ToRadians(Projectile.ai[1] == 1 ? -size - 10 : size - 10) * -Owner.direction, swoosh.Size() * 0.5f, Projectile.scale * 0.55f, SpriteEffects.None);


                Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), tex.Frame(1, FrameCount, 0, Frame), lightColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
            }
            return false;
        }
        public override void ResetStyle()
        {
        }
    }
}