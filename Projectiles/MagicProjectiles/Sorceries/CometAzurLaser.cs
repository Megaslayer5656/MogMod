using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Classes;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class CometAzurLaser : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public override string Texture => "MogMod/Projectiles/MagicProjectiles/Sorceries/CometAzurLaser";
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
        public override float MaxScale => 4f;
        public override float MaxLaserLength => 2700f;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/PhylacteryStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/PhylacteryMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/PhylacteryEnd", AssetRequestMode.ImmediateLoad).Value;
        private const float AimResponsiveness = 0.97f; // Last Prism is 0.92f. Lower makes the laser turn faster. if above 1.0 it turns the beam backwards
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = hitCooldown;
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
        // Update CutTiles so the laser will cut tiles (like grass).
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * LaserLength, Projectile.width + 16, DelegateMethods.CutTiles);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // If the beam doesn't have a defined direction, don't draw anything.
            if (Projectile.velocity == Vector2.Zero)
                return false;

            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            float drawArea = Projectile.localAI[1];
            Projectile projectile2 = Main.projectile[(int)Projectile.ai[1]];
            Color color = MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / ColorSet.Length % 1f, ColorSet);
            Color color2 = MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / ColorSet2.Length % 1f, ColorSet2);


            Vector2 drawStart = Projectile.Center.Floor();
            drawStart += Projectile.velocity * Projectile.scale * 10.5f;
            drawArea -= Projectile.scale * 14.5f * Projectile.scale;
            Vector2 drawScale = new Vector2(Projectile.scale);
            DelegateMethods.f_1 = 1f;
            DelegateMethods.c_1 = color * 0.75f * Projectile.Opacity;
            Vector2 projPos = Projectile.oldPos[0];
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/CircleGradient").Value;

            if (Main.zenithWorld)
            {
                Main.spriteBatch.SetBlendState(BlendState.Additive);
                Vector2 drawPosition = Owner.Center + bloomTex.Size() * 0f - Main.screenPosition;
                for (int n = 0; n < 4; n++)
                    Main.EntitySpriteDraw(bloomTex, drawPosition, null, color2, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.4f, SpriteEffects.None);
                return false;
            }
            projPos = new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Utils.DrawLaser(Main.spriteBatch, tex, drawStart - Main.screenPosition, drawStart + Projectile.velocity * drawArea - Main.screenPosition, drawScale, new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw));
            DelegateMethods.c_1 = color * 0.75f * Projectile.Opacity;
            for (int i = 0; i < 2; i++)
                Utils.DrawLaser(Main.spriteBatch, tex, drawStart - Main.screenPosition, drawStart + Projectile.velocity * drawArea - Main.screenPosition, drawScale / i, new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw));
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (int i = 0; i < 2; i++)
            {
                Vector2 drawPosition = Projectile.Center + bloomTex.Size() * 0f - Main.screenPosition;
                for (int n = 0; n < 2; n++)
                {
                    Main.EntitySpriteDraw(bloomTex, drawPosition, null, color2, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.4f, SpriteEffects.None);
                }
            }
            return false;
        }
    }
}