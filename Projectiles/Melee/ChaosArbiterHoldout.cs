using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.Projectiles.Melee
{
    public class ChaosArbiterHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override int swingWidth => 240;
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<ChaosArbiter>()).Item;
        public override LocalizedText DisplayName => MiscUtils.GetItemName<ChaosArbiter>();
        public override string Texture => ModContent.GetModItem(BaseItem.type).Texture;
        public override int OffsetDistance => 50;
        public override int StartupTime { get; set; }
        public override int CooldownTime { get; set; }
        //public override bool AlternateSwings => false;
        public override float lineCollisionLength => 32;
        public Player Owner => Main.player[Projectile.owner];
        public override SoundStyle? UseSound => SoundID.Item1;
        public bool playSwingSound = true;
        public bool ultraCrit = false;
        public override void SetStaticDefaults() => Main.projFrames[Projectile.type] = 9;
        public override void Defaults()
        {
            Projectile.extraUpdates = 5;
            Projectile.hide = true;
        }
        public override void Spawn()
        {
            Projectile.numHits = 0;
            StartupTime = 12;
            CooldownTime = 10;
            swingTime -= StartupTime + CooldownTime + 10;
            Projectile.scale *= 1.5f;
            ultraCrit = Main.rand.NextFloat(0f, 1f) < ChaosArbiter.UltraCritChance;
            RotateInStartup = 1;
        }
        public override void AdditionalAI()
        {
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            if (playSwingSound && !inStartup)
            {
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.9f, Pitch = Main.rand.NextFloat(0.1f, 0f) }, Projectile.Center);
                playSwingSound = false;
                if (Main.rand.NextFloat(0f, 1f) < ChaosArbiter.BoltChance)
                {
                    Vector2 position = ProjectilePosition != Vector2.Zero ? ProjectilePosition : Owner.Center;
                    Vector2 aimVel = (position - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, -(aimVel / 4), ModContent.ProjectileType<ChaosBoltProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Main.rand.Next(0, 6)); // 5 types of bolts
                }
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter % (Projectile.extraUpdates * 5) == 0)
                Projectile.frame = Projectile.frame >= 8 ? 0 : Projectile.frame + 1;

            if (inStartup) Projectile.scale = baseScale * MathHelper.Lerp(0.5f, 1, 1 - MathF.Pow(1 - StartupCompletion, 2f));
            else if (inCooldown) Projectile.scale = baseScale * MathHelper.Lerp(1, 0.75f, MathF.Pow(CooldownCompletion, 2));
            else
            {
                Projectile.scale = baseScale * Math.Min(MathHelper.SmoothStep(1, 1.5f, SwingCompletion), MathHelper.SmoothStep(2, 1, SwingCompletion));

                var veloc = oldPlayerOffset - (Projectile.Center - Main.player[Projectile.owner].Center);
                veloc.Normalize();
                float maxRotationDeviance = 0.8f;
                float rotationAngle = Main.rand.NextFloat(-maxRotationDeviance, maxRotationDeviance);
                float scale = Main.rand.NextFloat(0.7f, 1.15f);
                Vector2 velocity = veloc.RotatedBy(MathHelper.PiOver4 * 0.5f * Projectile.spriteDirection) * Main.rand.NextFloat(1, 4);

                Vector2 dustVel = new Vector2(-20 * Projectile.spriteDirection, -5).RotatedBy(Projectile.rotation);
                for (int i = 0; i < 1; i++)
                {
                    if (ProjectilePosition != Vector2.Zero && Main.rand.NextBool())
                        break;
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + dustVel.RotatedByRandom(0.4f) * Projectile.scale, DustID.FireworksRGB, velocity, 100, Color.WhiteSmoke, scale);
                    dust2.velocity *= 1.05f;
                    if (Main.rand.NextBool(4)) dust2.velocity *= 1.85f;
                    dust2.scale *= Main.rand.NextFloat(0.75f, 1.05f);
                    if (Main.rand.NextBool(4)) dust2.scale *= Main.rand.NextFloat(0.25f, 1.65f);
                    dust2.noGravity = true;
                    if (Main.rand.NextBool(2)) dust2.noGravity = false;
                    dust2.color = Color.Lerp(Color.Red, Color.OrangeRed, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f);
                }

                for (int i = 0; i < 1; i++)
                {
                    if (ProjectilePosition != Vector2.Zero && Main.rand.NextBool(2))
                        break;
                    Dust outerDust = Dust.NewDustPerfect(Projectile.Center + new Vector2(-angle.X.DirectionalSign(), Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation - 0.7f * Projectile.spriteDirection) * (Main.rand.NextFloat(-30, -45) * (mogPlayer.swingNum % 2 == 0 ? -1f : 1f)) * Projectile.scale, DustID.FireworksRGB, velocity, 100, Color.WhiteSmoke, scale);
                    outerDust.scale *= Main.rand.NextFloat(0.75f, 1.05f);
                    if (Main.rand.NextBool(4)) outerDust.scale *= Main.rand.NextFloat(0.25f, 1.65f);
                    outerDust.noGravity = true;
                    outerDust.color = Color.Lerp(Color.LightGoldenrodYellow, Color.IndianRed, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f);
                }
            }
        }
        public override float SwingFunction()
        {
            if (inStartup) return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * -0.2f, swingWidth * -0.6f, StartupCompletion));
            if (inCooldown) return MathHelper.ToRadians(MathHelper.Lerp(swingWidth * 0.2f, swingWidth * 0.33f, 1 - MathF.Pow(1 - CooldownCompletion, 3f)));
            return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * -0.6f, swingWidth * 0.2f, SwingCompletion));
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if ((target.life <= 0 && target.realLife == -1) && Projectile.numHits <= 2)
                Projectile.numHits -= 1;
            if (Projectile.numHits < 3)
            {
                if (ultraCrit && hit.Crit)
                {
                    if (target.type != NPCID.TargetDummy)
                    {
                        int heal = Main.rand.Next(1, 5 + 1);
                        Owner.HealLifestealMult(heal);
                    }
                    if (Owner.ownedProjectileCounts[ModContent.ProjectileType<ChaosArbiterClone>()] <= ChaosArbiter.MaxPhantoms && ProjectilePosition == Vector2.Zero)
                    {
                        Projectile clone = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, Vector2.Zero, ModContent.ProjectileType<ChaosArbiterClone>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        clone.OriginalCritChance = Owner.HeldItem.crit;
                    }
                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)MogModMessageType.UltraCritTextSync);
                        packet.Write(target.lastInteraction);
                        packet.Write(target.whoAmI);
                        packet.Send();
                    }
                    else
                        target.MogMod().UltraCritFX(target);
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.SourceDamage *= Main.rand.NextFloat(0.5f, 2f);
            modifiers.CritDamage += Main.rand.NextFloat(-0.75f, 1.25f);
            if (Main.rand.NextBool(3)) modifiers.Knockback *= Main.rand.NextFloat(0f, 1f);
            else modifiers.Knockback += Main.rand.Next(0, 3);
            if (Main.rand.Next(0, 100 + 1) < (Owner.GetTotalCritChance(Projectile.DamageType) * Main.rand.Next(0, 5 + 1))) modifiers.SetCrit();
            if (ultraCrit) modifiers.CritDamage *= ChaosArbiter.CritMult;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Projectiles/Melee/ChaosArbiterGhost").Value;
            float outlineWidth = 4;
            if (!inCooldown)
                outlineWidth *= 1 - SwingCompletion;
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.25f)
            {
                Main.spriteBatch.Draw(ghost,
                    Projectile.Center + new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX.RotatedBy(i + Projectile.rotation) * outlineWidth * Projectile.scale - Main.screenPosition,
                    null,
                    Color.Lerp(Color.Red, Color.OrangeRed, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f),
                    Projectile.rotation,
                    ghost.Size() * 0.5f,
                    Projectile.scale,
                    Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                    0);
            }
            return true;
        }
    }
}