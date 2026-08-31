using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Weapons.Magic;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Melee;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class MichaelSwordHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override int swingWidth => 220;
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<MichaelSword>()).Item;
        public override LocalizedText DisplayName => MiscUtils.GetItemName<MichaelSword>();
        public override string Texture => ModContent.GetModItem(BaseItem.type).Texture;
        public override int AfterImageLength => 12;
        public override int OffsetDistance => 50;
        public override int StartupTime { get; set; }
        public override int CooldownTime { get; set; }
        public override bool AlternateSwings => false;
        public override float lineCollisionLength => 52;
        public Player Owner => Main.player[Projectile.owner];
        public override SoundStyle? UseSound => SoundID.Item1;
        public override bool FlipHoldoutSprite => true;
        public bool playSwingSound = true;
        public float bladefx = 0;
        public override void Defaults()
        {
            Projectile.extraUpdates = 2;
            Projectile.hide = true;
        }
        public override void Spawn()
        {
            Projectile.numHits = 0;
            StartupTime = 25;
            CooldownTime = 10;
            swingTime -= StartupTime + CooldownTime + 10;
        }
        public override void AdditionalAI()
        {
            if (playSwingSound && !inStartup)
            {
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.9f, Pitch = Main.rand.NextFloat(0.1f, 0f) }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item94, Projectile.Center);
                Vector2 position = ProjectilePosition != Vector2.Zero ? ProjectilePosition : Owner.Center;
                Vector2 aimVel = (position - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, -(aimVel / 4), ModContent.ProjectileType<MichaelSwordBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                playSwingSound = false;
            }
            if (inStartup) Projectile.scale = baseScale * MathHelper.Lerp(0.5f, 1, 1 - MathF.Pow(1 - StartupCompletion, 2f));
            else if (inCooldown) Projectile.scale = baseScale * MathHelper.Lerp(1, 0.75f, MathF.Pow(CooldownCompletion, 2));
            else
            {
                bladefx = MathHelper.Lerp(bladefx, 1, 0.25f / Projectile.MaxUpdates * Owner.GetTotalAttackSpeed(Projectile.DamageType));
                Projectile.scale = baseScale * Math.Min(MathHelper.SmoothStep(1, 1.5f, SwingCompletion), MathHelper.SmoothStep(2, 1, SwingCompletion));

                var veloc = oldPlayerOffset - (Projectile.Center - Main.player[Projectile.owner].Center);
                veloc.Normalize();
                float maxRotationDeviance = 0.8f;
                float rotationAngle = Main.rand.NextFloat(-maxRotationDeviance, maxRotationDeviance);
                float scale = Main.rand.NextFloat(0.7f, 1.15f);
                Vector2 velocity = veloc.RotatedBy(MathHelper.PiOver4 * 0.5f * Projectile.spriteDirection) * Main.rand.NextFloat(1, 4);
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustVel = new Vector2(10 * Projectile.spriteDirection, -50).RotatedBy(Projectile.rotation);
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + dustVel.RotatedByRandom(0.4f) * Projectile.scale, DustID.FireworksRGB, velocity, 100, Color.WhiteSmoke, scale);
                    dust2.velocity *= 1.05f;
                    if (Main.rand.NextBool(4)) dust2.velocity *= 1.85f;
                    dust2.scale *= Main.rand.NextFloat(0.75f, 1.05f);
                    if (Main.rand.NextBool(4)) dust2.scale *= Main.rand.NextFloat(0.25f, 1.65f);
                    dust2.noGravity = true;
                    if (Main.rand.NextBool(4)) dust2.noGravity = false;
                    dust2.color = Color.Lerp(Color.SkyBlue, Color.LightSkyBlue, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f);
                }
            }
        }
        public override float SwingFunction()
        {
            if (inStartup) return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * -0.2f, swingWidth * 0.65f, StartupCompletion));
            if (inCooldown) return MathHelper.ToRadians(MathHelper.Lerp(swingWidth * -0.2f, swingWidth * -0.33f, 1 - MathF.Pow(1 - CooldownCompletion, 3f)));
            return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * 0.65f, swingWidth * -0.2f, SwingCompletion));
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/MichaelSwordGhost").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float outlineWidth = 5;
            if (!inCooldown)
                outlineWidth *= 1 - SwingCompletion;
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.25f)
            {
                Main.spriteBatch.Draw(ghost,
                    drawPosition + new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX.RotatedBy(i + Projectile.rotation) * outlineWidth * Projectile.scale,
                    null,
                    Color.Lerp(Color.SkyBlue, Color.LightSkyBlue, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f),
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