using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.MogModPlayer;
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
    public class MoonveilHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override int swingWidth => 250;
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<Moonveil>()).Item;
        public override LocalizedText DisplayName => MiscUtils.GetItemName<Moonveil>();
        public override string Texture => ModContent.GetModItem(BaseItem.type).Texture;
        public override int OffsetDistance => 50;
        public override int StartupTime { get; set; }
        public override int CooldownTime { get; set; }
        public override float lineCollisionLength => 52;
        public Player Owner => Main.player[Projectile.owner];
        public override SoundStyle? UseSound => SoundID.DD2_MonkStaffSwing with { Volume = 0.8f, Pitch = Main.rand.NextFloat(0.15f, 0.3f) };
        public float fadeIn = 0f;
        public bool playSwingSound = true;
        private int maxSwings = 3;
        public override void Defaults()
        {
            Projectile.width = 50;
            Projectile.height = 64;
            Projectile.extraUpdates = 5;
            Projectile.hide = true;
        }
        public override void Spawn()
        {
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            if (Main.myPlayer == Projectile.owner)
                mogPlayer.swingNum = mogPlayer.swingNum++ % maxSwings;
            StartupTime = mogPlayer.swingNum % maxSwings == 0 ? 5 : 7;
            CooldownTime = mogPlayer.swingNum % maxSwings == (maxSwings - 1) ? 3 : 5;
            swingTime -= mogPlayer.swingNum % maxSwings == 0 ? StartupTime : StartupTime - CooldownTime;
            Projectile.scale *= mogPlayer.swingNum % maxSwings == 0 ? 2f : 1.35f;
            Projectile.scale *= Owner.HasBuff<ParryBuff1>() ? 1.25f : 1f;
        }
        public override void AdditionalAI()
        {
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            switch (mogPlayer.swingNum)
            {
                // giant fast swing
                case 0:
                    if (!inStartup)
                    {
                        if (playSwingSound)
                        {
                            SoundEngine.PlaySound(SoundID.Item15 with { Volume = 1.0f, Pitch = Main.rand.NextFloat(0.05f, 0.25f) }, Projectile.Center);
                            if (Moonveil.Charges > 0)
                            {
                                Moonveil.Charges--;
                                Vector2 aimVel = (Owner.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                                if (Projectile.owner == Main.myPlayer)
                                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, -(aimVel / 4), ModContent.ProjectileType<MoonveilProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                            }
                            playSwingSound = false;
                        }
                    }
                    break;
                // 1st swing
                case 1:
                    if (Moonveil.Charges > 0)
                        if (playSwingSound && !inStartup)
                        {
                            Moonveil.Charges--;
                            SoundEngine.PlaySound(SoundID.Item15 with { Volume = 0.8f, Pitch = Main.rand.NextFloat(0.25f, 0.45f) }, Projectile.Center);
                            playSwingSound = false;
                        }
                    break;
                // 2nd swing
                case 2:
                    if (Moonveil.Charges > 0)
                        if (playSwingSound && !inStartup)
                        {
                            Moonveil.Charges--;
                            SoundEngine.PlaySound(SoundID.Item15 with { Volume = 0.8f, Pitch = Main.rand.NextFloat(0.15f, 0.35f) }, Projectile.Center);
                            playSwingSound = false;
                        }
                    break;
            }
            fadeIn = MathHelper.Lerp(fadeIn, 1, 0.25f / Projectile.MaxUpdates * 5);

            var veloc = oldPlayerOffset - (Projectile.Center - Main.player[Projectile.owner].Center);
            veloc.Normalize();
            float maxRotationDeviance = 0.4f;
            float rotationAngle = Main.rand.NextFloat(-maxRotationDeviance, maxRotationDeviance);
            float scale = Main.rand.NextFloat(0.7f, 1.15f);
            Vector2 velocity = veloc.RotatedBy(MathHelper.PiOver4 * 0.5f * Projectile.spriteDirection) * Main.rand.NextFloat(2, 5);
            for (int i = 0; i < (Moonveil.Charges > 0 ? 2 : 1); i++)
            {
                if (Moonveil.Charges <= 0 && Main.rand.NextBool(2))
                    break;
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center + new Vector2(-angle.X.DirectionalSign(), Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation - 0.7f * Projectile.spriteDirection) * (Main.rand.NextFloat(-10, -35) * (mogPlayer.swingNum % 2 == 0 ? -1f : 1f)) * Projectile.scale, DustID.FireworksRGB, velocity, 100, Color.WhiteSmoke, scale);
                dust2.scale *= Main.rand.NextFloat(0.75f, 1.05f);
                if (Main.rand.NextBool(4)) dust2.scale *= Main.rand.NextFloat(0.25f, 1.65f);
                dust2.noGravity = true;
                if (Main.rand.NextBool(4)) dust2.noGravity = false;
                dust2.color = Color.Lerp(Color.LightGoldenrodYellow, Color.LightBlue, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f);
            }
        }
        public override float SwingFunction()
        {
            if (inStartup) return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.6f, -swingWidth * 0.5f, MathF.Pow(StartupCompletion, 2f)));
            if (inCooldown) return MathHelper.ToRadians(MathHelper.Lerp(swingWidth * 0.5f, swingWidth * 0.6f, 1 - MathF.Pow(1 - CooldownCompletion, 2f)));
            return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.5f, swingWidth * 0.5f, SwingCompletion));
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            if (mogPlayer.swingNum == 0)
            {
                modifiers.SourceDamage *= 2.2f;
                modifiers.Knockback += 0.5f;
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);
        public override bool PreDraw(ref Color lightColor)
        {
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            if (Moonveil.Charges > 0 || mogPlayer.swingNum == 0)
            {
                Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Projectiles/Melee/MoonveilGhost").Value;
                float outlineWidth = 4;
                if (!inCooldown)
                    outlineWidth *= 1 - SwingCompletion;
                for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.25f)
                {
                    Main.spriteBatch.Draw(ghost,
                        Projectile.Center + new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX.RotatedBy(i + Projectile.rotation) * outlineWidth * Projectile.scale - Main.screenPosition,
                        null,
                        Color.Lerp(Color.LightGoldenrodYellow, Color.LightBlue, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f),
                        Projectile.rotation,
                        ghost.Size() * 0.5f,
                        Projectile.scale,
                        Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                        0);
                }
            }
            return true;
        }
    }
}