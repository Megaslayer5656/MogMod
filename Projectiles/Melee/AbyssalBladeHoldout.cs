using Microsoft.Xna.Framework;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.Projectiles.Melee
{
    public class AbyssalBladeHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override int swingWidth => 300;
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<AbyssalBlade>()).Item;
        public override LocalizedText DisplayName => MiscUtils.GetItemName<AbyssalBlade>();
        public override string Texture => ModContent.GetModItem(BaseItem.type).Texture;
        public override int AfterImageLength => 18;
        public override int OffsetDistance => 50;
        public override int StartupTime { get; set; }
        public override int CooldownTime { get; set; }
        public override bool AlternateSwings => false;
        public override float lineCollisionLength => 82;
        public Player Owner => Main.player[Projectile.owner];
        public override SoundStyle? UseSound => SoundID.Item1;
        public bool playSwingSound = true;
        public override void Defaults()
        {
            Projectile.extraUpdates = 2;
        }
        public override void Spawn()
        {
            Projectile.numHits = 0;
            StartupTime = 5;
            CooldownTime = 2;
            swingTime -= StartupTime - CooldownTime;
            Projectile.scale *= 2f;
        }
        public override void AdditionalAI()
        {
            if (playSwingSound && !inStartup)
            {
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.9f, Pitch = Main.rand.NextFloat(-0.15f, -0.3f) }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_SonicBoomBladeSlash with { Volume = 1.0f, Pitch = Main.rand.NextFloat(-0.15f, -0.35f) }, Projectile.Center);
                playSwingSound = false;
            }
            var veloc = oldPlayerOffset - (Projectile.Center - Main.player[Projectile.owner].Center);
            veloc.Normalize();
            float maxRotationDeviance = 0.8f;
            float rotationAngle = Main.rand.NextFloat(-maxRotationDeviance, maxRotationDeviance);
            float scale = Main.rand.NextFloat(0.7f, 1.15f);
            Vector2 velocity = veloc.RotatedBy(MathHelper.PiOver4 * 0.5f * Projectile.spriteDirection) * Main.rand.NextFloat(2, 5);
            for (int i = 0; i < 2; i++)
            {
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center + new Vector2(-angle.X.DirectionalSign(), Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation - 0.7f * Projectile.spriteDirection) * Main.rand.NextFloat(-10, -40) * -1f * Projectile.scale, DustID.AncientLight, velocity, 100, Color.WhiteSmoke, scale);
                dust2.velocity *= 1.05f;
                if (Main.rand.NextBool(4))
                    dust2.velocity *= 1.85f;
                dust2.scale *= Main.rand.NextFloat(0.75f, 1.05f);
                if (Main.rand.NextBool(4))
                    dust2.scale *= Main.rand.NextFloat(0.25f, 1.65f);
                dust2.noGravity = true;
                if (Main.rand.NextBool(4))
                    dust2.noGravity = false;
                dust2.color = Color.Lerp(Color.DarkRed, Color.IndianRed, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f);
            }
            if (inStartup)
                Projectile.scale = baseScale * MathHelper.Lerp(0.5f, 1, 1 - MathF.Pow(1 - StartupCompletion, 2f));
            else if (inCooldown)
                Projectile.scale = baseScale * MathHelper.Lerp(1, 0.75f, MathF.Pow(CooldownCompletion, 2));
            else
                Projectile.scale = baseScale * Math.Min(MathHelper.SmoothStep(1, 1.5f, SwingCompletion), MathHelper.SmoothStep(2, 1, SwingCompletion));
        }
        public override float SwingFunction()
        {
            if (inStartup)
                return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.6f, -swingWidth * 0.5f, MathF.Pow(StartupCompletion, 2f)));
            if (inCooldown)
                return MathHelper.ToRadians(MathHelper.Lerp(swingWidth * 0.5f, swingWidth * 0.6f, 1 - MathF.Pow(1 - CooldownCompletion, 2f)));
            return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.5f, swingWidth * 0.5f, SwingCompletion));
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = Owner.GetSource_OnHit(target);
            if ((target.life <= 0 && target.realLife == -1) && Projectile.numHits <= 2)
                Projectile.numHits -= 1;
            if (Projectile.numHits < 3)
            {
                if (Main.rand.NextBool(4) && Projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        MogModUtils.ProjectileBarrage(source, target.Center, target.Center, Main.rand.NextBool(), 50f, 50f, -50f, 100f, 0.25f, ModContent.ProjectileType<AbyssalBladeProj>(), (int)(Projectile.damage * 0.5f), 0f, Owner.whoAmI, false, 0f);
                    }
                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)MogModMessageType.BashProcTextSync);
                        packet.Write(target.lastInteraction);
                        packet.Write(target.whoAmI);
                        packet.Send();
                    }
                    else
                    {
                        target.MogMod().BashFX(target);
                    }
                }
            }
        }
        /* TODO: resprite, then add this pre-draw
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Projectiles/Melee/AbyssalBladeGlow").Value;

            float outlineWidth = 4;
            if (!inCooldown)
            {
                outlineWidth *= 1 - SwingCompletion;
            }
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.25f)
            {
                Main.spriteBatch.Draw(ghost,
                    Projectile.Center + new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX.RotatedBy(i + Projectile.rotation) * outlineWidth * Projectile.scale - Main.screenPosition,
                    null,
                    Color.Lerp(Color.DarkRed, Color.IndianRed, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f),
                    Projectile.rotation,
                    ghost.Size() * 0.5f,
                    Projectile.scale,
                    Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                    0);
            }
            return true;
        }
        */
    }
}