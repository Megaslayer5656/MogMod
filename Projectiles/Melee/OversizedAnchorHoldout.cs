using Microsoft.Xna.Framework;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Classless;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class OversizedAnchorHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<OversizedAnchor>()).Item;
        public override LocalizedText DisplayName => MiscUtils.GetItemName<OversizedAnchor>();
        public override string Texture => ModContent.GetModItem(BaseItem.type).Texture;
        public override int AfterImageLength => 10;
        public override int OffsetDistance => 80;
        public override int StartupTime { get; set; }
        public override int CooldownTime { get; set; }
        public Player Owner => Main.player[Projectile.owner];
        public override SoundStyle? UseSound => SoundID.Item1;
        public bool playSwingSound = true;
        public bool hitNPC = false;
        public bool hasSmashedTile = false;
        public bool summonProj = true;
        public override void Defaults()
        {
            Projectile.extraUpdates = 3;
            Projectile.width = Projectile.height = 100;
        }
        public override void Spawn()
        {
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            if (Main.myPlayer == Projectile.owner) mogPlayer.swingNum = mogPlayer.swingNum++ % 3;
            bool secondSwing = mogPlayer.swingNum % 3 == 2;
            bool thirdSwing = mogPlayer.swingNum % 3 == 0;
            StartupTime = thirdSwing ? 12 : secondSwing ? 4 : 12;
            CooldownTime = thirdSwing ? 12 : secondSwing ? 12 : 4;
            swingTime -= secondSwing ? -StartupTime : StartupTime - CooldownTime;
            Projectile.scale *= thirdSwing ? 1.4f : secondSwing ? 1.2f : 1f;
            swingWidth = mogPlayer.swingNum % 3 == 0 ? -360 : 270;
        }
        public override void AdditionalAI()
        {
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            Vector2 aimVel = (Owner.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
            switch (mogPlayer.swingNum)
            {
                // spin
                case 0:
                    if (playSwingSound && !inStartup)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.8f, Pitch = Main.rand.NextFloat(-0.05f, -0.25f) }, Projectile.Center);
                        playSwingSound = false;
                    }
                    if (SwingCompletion > 0.275f && summonProj)
                    {
                        SoundEngine.PlaySound(SoundID.Item107);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Vector2.Zero, ModContent.ProjectileType<AnchorSmashProj>(), (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner);
                        summonProj = false;
                    }
                    break;
                // small swing
                case 1:
                    if (playSwingSound && !inStartup)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.8f, Pitch = Main.rand.NextFloat(-0.05f, -0.25f) }, Projectile.Center);
                        playSwingSound = false;
                    }
                    if (SwingCompletion > 0.275f && summonProj)
                    {
                        if (Projectile.owner == Main.myPlayer)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, -(aimVel / 4), ModContent.ProjectileType<AnchorProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        summonProj = false;
                    }
            break;
                // big swing
                case 2:
                    if (SwingCompletion > 0.275f && summonProj)
                    {
                        for (float i = -0.25f; i < 0.26f; i += 0.50f)
                        {
                            if (Projectile.owner == Main.myPlayer)
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, -(aimVel / 4).RotatedBy(i), ModContent.ProjectileType<AnchorProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                        summonProj = false;
                    }
                    if (playSwingSound && !inStartup)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.8f, Pitch = Main.rand.NextFloat(-0.05f, -0.25f) }, Projectile.Center);
                        playSwingSound = false;
                    }
                    if (!hasSmashedTile && inSwing && SwingCompletion > 0.275f)
                    {
                        var adjustedAngle = angle.RotatedBy(Projectile.spriteDirection * SwingFunction());
                        Vector2 HammerFrontPos = Projectile.Center + adjustedAngle * -16 * Projectile.scale + (adjustedAngle.RotatedBy(MathHelper.PiOver2) * 20 * Projectile.scale * angle.X);
                        if (Collision.SolidCollision(HammerFrontPos, 1, 1))
                        {
                            Owner.velocity *= 0.15f;
                            Owner.velocity -= adjustedAngle.RotatedBy(MathHelper.PiOver2) * angle.X * 3f;
                            float ringRot = SwingCompletion < 0.5f ? 0 : MathHelper.PiOver2;
                            int radius = 8;
                            Point scanAreaStart = HammerFrontPos.ToTileCoordinates() + new Point(-radius, -radius);
                            Point scanAreaEnd = HammerFrontPos.ToTileCoordinates() + new Point(radius, radius);
                            Projectile.CreateImpactExplosion(radius - 1, Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out bool causedShockwaves);

                            hasSmashedTile = true;
                            timer = StartupTime + swingTime;
                            angle = adjustedAngle;
                            var pos = Projectile.Center;
                            Projectile.Size *= (float)(radius * 0.1f) + 1;
                            Projectile.Center = HammerFrontPos;
                            Projectile.Damage();
                            Projectile.Size /= (float)(radius * 0.1f) + 1;
                            Projectile.Center = pos;
                            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact);
                            SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack with { Volume = 0.5f, LimitsArePerVariant = false, MaxInstances = 1 });
                        }
                    }
                    if (inCooldown) mogPlayer.swingNum = 0;
                    Owner.heldProj = Projectile.whoAmI;
                    break;
            }
            if (inStartup) Projectile.scale = baseScale * MathHelper.Lerp(0.5f, 1, 1 - MathF.Pow(1 - StartupCompletion, 2f));
            else if (inCooldown) Projectile.scale = baseScale * MathHelper.Lerp(1, 0.75f, MathF.Pow(CooldownCompletion, 2));
            else Projectile.scale = baseScale * Math.Min(MathHelper.SmoothStep(1, 1.5f, SwingCompletion), MathHelper.SmoothStep(2, 1, SwingCompletion));
        }
        public override float SwingFunction()
        {
            if (hasSmashedTile) return MathHelper.ToRadians(MathHelper.Lerp(0, -swingWidth * 0.4f, MathF.Pow(CooldownCompletion, 0.5f)));
            if (inStartup) return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.5f, -swingWidth * 0.75f, 1 - MathF.Pow(1 - StartupCompletion, 2f)));
            if (inCooldown) return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * 0.25f, swingWidth * 0.33f, CooldownCompletion));
            return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.75f, swingWidth * 0.25f, SwingCompletion));
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            if (mogPlayer.swingNum == 2)
            {
                modifiers.SourceDamage *= 2.2f;
                modifiers.Knockback += 1;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            if (Projectile.numHits < 3 && mogPlayer.swingNum != 0)
            {
                for (int i = 0; i < Main.rand.Next(4, 7); i++)
                {
                    MogModUtils.ProjectileBarrage(Projectile.GetSource_FromThis(), Projectile.Center, target.Center, Main.rand.NextBool(), 200f, 200f, -200f, 200f, 6f, ModContent.ProjectileType<AnchorProj>(), (int)(Projectile.damage * 0.5f), 3f, Projectile.owner, false, 0f);
                }
            }
        }
    }
}