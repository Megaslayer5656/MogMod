using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Classes;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class CometAzurLaser : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        private const string LaserTexturePath = "MogMod/Projectiles/MagicProjectiles/Sorceries/CometAzurLaser";
        public override Texture2D LaserBeginTexture => Request<Texture2D>(LaserTexturePath + "Start").Value;
        public override Texture2D LaserMiddleTexture => Request<Texture2D>(LaserTexturePath + "Mid").Value;
        public override Texture2D LaserEndTexture => Request<Texture2D>(LaserTexturePath + "End").Value;
        public override string Texture => "MogMod/Projectiles/MagicProjectiles/Sorceries/CometAzurLaserStart";
        public static readonly SoundStyle laserSound = new SoundStyle("Terraria/Sounds/Item_15")
        {
            Volume = 1f,
            PitchVariance = 0.2f,
            MaxInstances = -1
        };
        private const float BeamRenderTileOffset = 10.5f;
        private const float BeamLengthReductionFactor = 14.5f;
        private const float OpacityMultiplier = 0.75f;
        private const int hitCooldown = 1;
        public static readonly Color[] ColorSet =
        [
            new Color(28, 255, 225, 50),
            new Color(32, 28, 255, 50),
            new Color(28, 157, 255, 50),
        ];
        public static readonly Color[] ColorSet2 =
        [
            new Color(110, 255, 178),
            new Color(134, 110, 255),
            new Color(110, 192, 255),
        ];
        public static Color Colour => new(134, 110, 255);
        public bool PlayedSound = false;
        public const int ChargeupTime = 60;
        public Player Owner => Main.player[Projectile.owner];
        public override Color LaserOverlayColor => MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / ColorSet.Length % 1f, ColorSet); // determines color from ColorSet array
        public override Color LightCastColor => LaserOverlayColor; // applies it
        public override float Lifetime => 3600f;
        public override float MaxScale => 3f;
        public override float MaxLaserLength => 1500f;
        private const float AimResponsiveness = 0.97f; // Last Prism is 0.92f. Lower makes the laser turn faster. if above 1.0 it turns the beam backwards
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = hitCooldown;
            Projectile.alpha = 255;
        }
        public override void DetermineScale()
        {
            Projectile.scale = Time < ChargeupTime ? 0f : Utils.GetLerpValue(0f, 40f, Projectile.timeLeft, true) * MaxScale;
        }
        public override bool PreAI()
        {
            // Multiplayer support here, only run this code if the client running it is the owner of the projectile
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 rrp = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
                UpdateAim(rrp);
                Projectile.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                Projectile.netUpdate = true;
            }

            int dir = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.Center = Owner.Center + Projectile.velocity * 50f;
            Owner.ChangeDir(dir);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = ((Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * -Owner.direction).ToRotation();

            if (!Owner.channel)
            {
                Projectile.Kill();
                return false;
            }

            // Do we still have enough mana? If not, we kill the projectile because we cannot use it anymore
            if (Owner.miscCounter % (hitCooldown * 3) == 0)
            {
                if (!Owner.CheckMana(Owner.ActiveItem(), -1, true))
                {
                    Projectile.Kill();
                    return false;
                }
                else if (Time > ChargeupTime)
                {
                    if (Owner.miscCounter % (hitCooldown * 21) == 0)
                        SoundEngine.PlaySound(laserSound, Projectile.Center);
                    int type = ModContent.ProjectileType<CometAzurProj>();
                    Vector2 velocity = Projectile.velocity * 15;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - Projectile.velocity, velocity, type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }

            if (Time < ChargeupTime)
            {
                // Crate charge-up dust.
                int dustCount = (int)(Time / 20f);
                Vector2 spawnPos = Projectile.Center;
                for (int k = 0; k < dustCount + 1; k++)
                {
                    Dust dust = Dust.NewDustDirect(spawnPos, 1, 1, Main.rand.NextBool(3) ? DustID.MagnetSphere : DustID.ApprenticeStorm, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f);
                    dust.position += Main.rand.NextVector2Square(-10f, 10f);
                    dust.velocity = Main.rand.NextVector2Unit() * (10f - dustCount * 2f) / 10f;
                    dust.color = Colour;
                    dust.scale = Main.rand.NextFloat(0.5f, 1f);
                    dust.noGravity = true;
                }
                DetermineScale();
                Time++;
                return false;
            }
            if (!PlayedSound)
            {
                SoundEngine.PlaySound(SoundID.Zombie104, Projectile.Center);
                PlayedSound = true;
            }
            return true;
        }
        // Gently adjusts the aim vector of the laser to point towards the mouse. if AimResponsiveness is above 1, the beam is backwards
        private void UpdateAim(Vector2 source)
        {
            if (Main.zenithWorld)
                return;
            Vector2 aimVector = Vector2.Normalize(Main.MouseWorld - source);
            if (aimVector.HasNaNs())
                aimVector = -Vector2.UnitY;
            aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(Projectile.velocity), AimResponsiveness));

            if (aimVector != Projectile.velocity)
                Projectile.netUpdate = true;
            Projectile.velocity = aimVector;
        }
        public override bool? CanDamage() => Time >= ChargeupTime;
        public override bool ShouldUpdatePosition() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float tigz = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, 65f, ref tigz);
        }
        // Update CutTiles so the laser will cut tiles (like grass).
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * LaserLength, Projectile.width + 16, DelegateMethods.CutTiles);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
                return false;

            Vector2 scale = new(Projectile.scale, Projectile.scale);

            Texture2D laserBegin = LaserBeginTexture;
            Texture2D laserMid = LaserMiddleTexture;
            Texture2D laserEnd = LaserEndTexture;

            float rayDrawLength = Projectile.localAI[1]; //length of laser
            Color baseColor = LaserOverlayColor;
            Vector2 vector = Projectile.Center - Main.screenPosition;
            Rectangle? sourceRectangle2 = null;
            if (!Main.zenithWorld)
                Main.spriteBatch.Draw(laserBegin, vector, sourceRectangle2, baseColor, Projectile.rotation, laserBegin.Size() / 2f, scale, SpriteEffects.None, 0);
            rayDrawLength -= (laserBegin.Height / 2 + laserEnd.Height) * Projectile.scale;
            Vector2 projCenter = Projectile.Center;
            projCenter += Projectile.velocity * Projectile.scale * laserBegin.Height / 2f;

            Texture2D GlowBallTexture = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/CircleGradient").Value;
            Texture2D GlowRingTexture = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/GlowRingParticle").Value;

            Projectile.localAI[2]++;

            Color glowColor = MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / ColorSet2.Length % 1f, ColorSet2);

            if (Main.zenithWorld)
            {
                for (int i = 0; i < 2; i++)
                {
                    Main.EntitySpriteDraw(GlowBallTexture, Owner.Center - Main.screenPosition, GlowBallTexture.Frame(), glowColor with { A = 0 }, 0f, GlowBallTexture.Frame().Center(), Projectile.scale / (2f + i), SpriteEffects.None);
                    Main.EntitySpriteDraw(GlowRingTexture, Owner.Center - Main.screenPosition, GlowRingTexture.Frame(), glowColor with { A = 0 }, 0f, GlowRingTexture.Frame().Center(), Projectile.scale / MathHelper.Lerp(2f + i, 2f / i, (float)Math.Sin(Projectile.localAI[2] / 20f) * 0.5f), SpriteEffects.None);
                }
                return false;
            }
            else
            { 
                for (int i = 0; i < 2; i++)
                {
                    Main.EntitySpriteDraw(GlowBallTexture, Projectile.Center - Main.screenPosition, GlowBallTexture.Frame(), glowColor with { A = 0 }, 0f, GlowBallTexture.Frame().Center(), Projectile.scale / (2f + i), SpriteEffects.None);
                    Main.EntitySpriteDraw(GlowRingTexture, Projectile.Center - Main.screenPosition, GlowRingTexture.Frame(), glowColor with { A = 0 }, 0f, GlowRingTexture.Frame().Center(), Projectile.scale / MathHelper.Lerp(2f + i, 2f / i, (float)Math.Sin(Projectile.localAI[2] / 20f) * 0.5f), SpriteEffects.None);
                }
            }

            if (rayDrawLength > 0f)
            {
                float raySegment = 0f;
                Rectangle drawRectangle = new Rectangle(0, 36 * (Projectile.timeLeft / 3 % 4), laserMid.Width, 36);
                while (raySegment + 1f < rayDrawLength)
                {
                    if (rayDrawLength - raySegment < drawRectangle.Height)
                        drawRectangle.Height = (int)(rayDrawLength - raySegment);

                    Main.spriteBatch.Draw(laserMid, projCenter - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(drawRectangle), baseColor, Projectile.rotation, new Vector2(drawRectangle.Width / 2, 0f), scale, SpriteEffects.None, 0);
                    raySegment += drawRectangle.Height * Projectile.scale;
                    projCenter += Projectile.velocity * drawRectangle.Height * Projectile.scale;
                    drawRectangle.Y += 36;

                    if (drawRectangle.Y + drawRectangle.Height > laserMid.Height)
                        drawRectangle.Y = 0;
                }
            }

            Vector2 vector2 = projCenter - Main.screenPosition;
            sourceRectangle2 = null;

            Main.spriteBatch.Draw(laserEnd, vector2, sourceRectangle2, baseColor, Projectile.rotation, laserEnd.Frame(1, 1, 0, 0).Top(), scale, SpriteEffects.None, 0);

            return false;
        }
    }
}