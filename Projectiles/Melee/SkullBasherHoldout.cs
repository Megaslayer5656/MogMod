using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.Projectiles.Melee
{
    public class SkullBasherHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public Player Owner => Main.player[Projectile.owner];
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<SkullBasher>()).Item;
        public override string Texture => BaseItem.ModItem.Texture;
        public override int AfterImageLength => 10;
        public override int OffsetDistance => 50;
        public override int CooldownTime { get; set; }
        public override SoundStyle? UseSound => SoundID.DD2_MonkStaffSwing with { Volume = 1f };
        bool hasSmashedTile = false;
        bool playedChargeSound = false;
        bool firstEnemyHit = true;
        public override void Defaults()
        {
            Projectile.extraUpdates = 3;
            swingWidth = 200;
            RotateInCooldown = 0;
            RotateInStartup = 0;
            Projectile.width = Projectile.height = 66;
        }
        public override void Spawn()
        {
            angle = new Vector2(angle.X.DirectionalSign(), 0);
            StartupTime = 20;
            CooldownTime = 30;
            swingTime = 10;
            Projectile.timeLeft = 600;
            Projectile.scale *= 1.25f;
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            mogPlayer.swingNum = 0;
        }
        public override void AdditionalAI()
        {
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            if (!hasSmashedTile && inSwing && SwingCompletion > 0.275f)
            {
                var adjustedAngle = angle.RotatedBy(Projectile.spriteDirection * SwingFunction());
                Vector2 HammerFrontPos = Projectile.Center + adjustedAngle * -16 * Projectile.scale + (adjustedAngle.RotatedBy(MathHelper.PiOver2) * 20 * Projectile.scale * angle.X);
                if (Collision.SolidCollision(HammerFrontPos, 1, 1))
                {
                    Owner.velocity *= 0.15f;
                    Owner.velocity -= adjustedAngle.RotatedBy(MathHelper.PiOver2) * angle.X * 3f;
                    float ringRot = SwingCompletion < 0.5f ? 0 : MathHelper.PiOver2;
                    int radius = 6;
                    Point scanAreaStart = HammerFrontPos.ToTileCoordinates() + new Point(-radius, -radius);
                    Point scanAreaEnd = HammerFrontPos.ToTileCoordinates() + new Point(radius, radius);
                    Projectile.CreateImpactExplosion(5, Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out bool causedShockwaves);

                    hasSmashedTile = true;
                    timer = StartupTime + swingTime;
                    angle = adjustedAngle;
                    var pos = Projectile.Center;
                    Projectile.Size *= 1.6f;
                    Projectile.Center = HammerFrontPos;
                    Projectile.Damage();
                    Projectile.Size /= 1.6f;
                    Projectile.Center = pos;
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact);
                }
            }
            Owner.heldProj = Projectile.whoAmI;
        }
        public override float SwingFunction()
        {
            if (hasSmashedTile)
                return MathHelper.ToRadians(MathHelper.Lerp(0, -swingWidth * 0.4f, MathF.Pow(CooldownCompletion, 0.5f)));
            if (inStartup)
                return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.8f, -swingWidth * 0.66f, 1 - MathF.Pow(StartupCompletion, 0.5f)));
            if (inCooldown)
                return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * 0.33f, swingWidth * 0.45f, MathF.Pow(CooldownCompletion, 0.5f)));
            return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * .66f, (swingWidth * 0.33f), SwingCompletion));
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hasSmashedTile)
            {
                SoundEngine.PlaySound(SoundID.Item69 with { Volume = 1f, LimitsArePerVariant = false, MaxInstances = 1 });
                var source = Owner.GetSource_OnHit(target);
                bool proc = Main.rand.NextBool(5);
                if (proc && firstEnemyHit)
                {
                    firstEnemyHit = false;
                    SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack with { Volume = 0.5f, LimitsArePerVariant = false, MaxInstances = 1 });

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
                for (int i = 0; i < (proc ? 85 : 40); i++)
                {
                    float scale = Main.rand.NextFloat(1f, 2f);
                    var color = Main.rand.NextBool() ? Color.Purple : Color.Red;

                    if (Main.rand.NextBool(5))
                        scale *= 1.4f;

                    Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * MathHelper.Lerp(2, 10, Main.rand.NextFloat());

                    int bash = Dust.NewDust(target.position, Projectile.width, Projectile.height, ChildSafety.Disabled ? DustID.Blood : DustID.CrimsonPlants, velocity.X, velocity.Y, 100, color, scale);
                    Main.dust[bash].fadeIn += 1.2f;
                    Main.dust[bash].velocity.Y *= 1.02f;
                    if (Main.rand.NextBool(4))
                        Main.dust[bash].velocity *= 0.25f;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!inCooldown)
            {
                var tex = ModContent.Request<Texture2D>(Texture).Value;
                float outlineWidth = 4;
                if (inSwing)
                {
                    outlineWidth *= 1 - SwingCompletion;
                    for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.25f)
                    {
                        Main.spriteBatch.Draw(tex,
                            Projectile.Center + new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX.RotatedBy(i + Projectile.rotation) * outlineWidth * Projectile.scale - Main.screenPosition,
                            null,
                            Color.Lerp(Color.Purple, Color.Red, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f),
                            Projectile.rotation,
                            tex.Size() * 0.5f,
                            Projectile.scale,
                            Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                            0
                        );
                    }
                }
            }
            if (inSwing)
                return base.PreDraw(ref lightColor);
            return true;
        }
    }
}