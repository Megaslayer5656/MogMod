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
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.Projectiles.Melee
{
    public class ChaosBladeHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override int swingWidth => 220;
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<ChaosBlade>()).Item;
        public override LocalizedText DisplayName => MiscUtils.GetItemName<ChaosBlade>();
        public override string Texture => ModContent.GetModItem(BaseItem.type).Texture;
        public override int AfterImageLength => 12;
        public override int OffsetDistance => 50;
        public override int StartupTime { get; set; }
        public override int CooldownTime { get; set; }
        public override bool AlternateSwings => false;
        public override float lineCollisionLength => 52;
        public Player Owner => Main.player[Projectile.owner];
        public override SoundStyle? UseSound => SoundID.Item1;
        public bool playSwingSound = true;
        public bool ultraCrit = false;
        public override void Defaults()
        {
            Projectile.extraUpdates = 2;
            Projectile.hide = true;
        }
        public override void Spawn()
        {
            Projectile.numHits = 0;
            StartupTime = 13;
            CooldownTime = 10;
            swingTime -= StartupTime + CooldownTime + 10;
            Projectile.scale *= 1.35f;
            ultraCrit = Main.rand.NextFloat(0f, 1f) < ChaosBlade.UltraCritChance;
        }
        public override void AdditionalAI()
        {
            if (playSwingSound && !inStartup)
            {
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.9f, Pitch = Main.rand.NextFloat(0.1f, 0f) }, Projectile.Center);
                playSwingSound = false;
            }
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
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustVel = new Vector2(20 * Projectile.spriteDirection, -5).RotatedBy(Projectile.rotation);
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + dustVel.RotatedByRandom(0.4f) * Projectile.scale, DustID.FireworksRGB, velocity, 100, Color.WhiteSmoke, scale);
                    dust2.velocity *= 1.05f;
                    if (Main.rand.NextBool(4))
                        dust2.velocity *= 1.85f;
                    dust2.scale *= Main.rand.NextFloat(0.75f, 1.05f);
                    if (Main.rand.NextBool(4))
                        dust2.scale *= Main.rand.NextFloat(0.25f, 1.65f);
                    dust2.noGravity = true;
                    if (Main.rand.NextBool(4))
                        dust2.noGravity = false;
                    dust2.color = Color.Lerp(Color.Red, Color.OrangeRed, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f);
                }
            }
        }
        public override float SwingFunction()
        {
            if (inStartup) return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * -0.2f, swingWidth * -0.65f, StartupCompletion));
            if (inCooldown) return MathHelper.ToRadians(MathHelper.Lerp(swingWidth * 0.2f, swingWidth * 0.33f, 1 - MathF.Pow(1 - CooldownCompletion, 3f)));
            return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * -0.65f, swingWidth * 0.2f, SwingCompletion));
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
            if (ultraCrit) modifiers.CritDamage *= ChaosBlade.CritMult;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Projectiles/Melee/ChaosBladeGhost").Value;
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