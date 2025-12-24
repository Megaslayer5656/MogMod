using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Projectiles;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles
{
    class KhandaBeam : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles";
        public override string Texture => "MogMod/Projectiles/KhandaBeam";

        public static readonly Color[] Colors = new Color[]
        {
            //new Color(255, 0, 0, 50), //Red
            //new Color(255, 128, 0, 50), //Orange
            //new Color(255, 255, 0, 50), //Yellow
            //new Color(128, 255, 0, 50), //Lime
            //new Color(0, 255, 0, 50), //Green
            //new Color(0, 255, 128, 50), //Turquoise
            //new Color(0, 255, 255, 50), //Cyan
            //new Color(0, 128, 255, 50), //Light Blue
            //new Color(0, 0, 255, 50), //Blue
            new Color(128, 0, 255, 50), //Purple
            new Color(255, 0, 255, 50), //Fuschia
            new Color(128, 0, 255, 50), //Purple
            //new Color(255, 0, 128, 50) //Hot Pink
        };
        public static readonly Color[] ColorSet = new Color[]
        {
            new Color(128, 0, 255, 50), //Purple
            new Color(255, 0, 255, 50), //Fuschia
            new Color(128, 0, 255, 50), //Purple
        };

        public bool PlayedSound = false;

        public const int ChargeupTime = 30;
        public Player Owner => Main.player[Projectile.owner];
        public override Color LaserOverlayColor => MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / ColorSet.Length % 1f, ColorSet); // determines color from ColorSet array
        public override Color LightCastColor => LaserOverlayColor; // applies it
        public override float Lifetime => 2700f;
        public override float MaxScale => 2.1f;
        public override float MaxLaserLength => 2200f;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/PhylacteryStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/PhylacteryMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("MogMod/Projectiles/PhylacteryEnd", AssetRequestMode.ImmediateLoad).Value;
        private const float AimResponsiveness = 0.88f; // Last Prism is 0.92f. Lower makes the laser turn faster. if above 1.0 it turns the beam backwards

        public override void SetDefaults()
        {
            Projectile.width = 7;
            Projectile.height = 7;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.scale = 2f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.timeLeft = 2700;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void DetermineScale()
        {
            Projectile.scale = Time < ChargeupTime ? 0f : Utils.GetLerpValue(0f, 40f, Projectile.timeLeft, true) * MaxScale;
        }

        public override float DetermineLaserLength()
        {
            return DetermineLaserLength_CollideWithTiles(5);
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
            if (Owner.miscCounter % 10 == 0 && !Owner.CheckMana(Owner.ActiveItem(), -1, true))
            {
                Projectile.Kill();
                return false;
            }

            if (Time < ChargeupTime)
            {
                // Crate charge-up dust.
                int dustCount = (int)(Time / 20f);
                Vector2 spawnPos = Projectile.Center;
                for (int k = 0; k < dustCount + 1; k++)
                {
                    Dust dust = Dust.NewDustDirect(spawnPos, 1, 1, DustID.Enchanted_Pink, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f);
                    dust.position += Main.rand.NextVector2Square(-10f, 10f);
                    dust.velocity = Main.rand.NextVector2Unit() * (10f - dustCount * 2f) / 10f;
                    // (Colors) <- uses Colors array at the top
                    dust.color = Main.rand.Next(Colors);
                    dust.scale = Main.rand.NextFloat(0.5f, 1f);
                    dust.noGravity = true;
                }
                DetermineScale();
                Time++;
                return false;
            }

            // Play a cool sound when fully charged.
            if (!PlayedSound)
            {
                SoundEngine.PlaySound(SoundID.Item68, Projectile.position);
                PlayedSound = true;
            }
            return true;
        }

        // Gently adjusts the aim vector of the laser to point towards the mouse. if AimResponsiveness is above 1, the beam is backwards
        private void UpdateAim(Vector2 source)
        {
            Vector2 aimVector = Vector2.Normalize(Main.MouseWorld - source);
            if (aimVector.HasNaNs())
                aimVector = -Vector2.UnitY;
            aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(Projectile.velocity), AimResponsiveness));

            if (aimVector != Projectile.velocity)
                Projectile.netUpdate = true;
            Projectile.velocity = aimVector;
        }

        public override bool ShouldUpdatePosition() => false;

        // Update CutTiles so the laser will cut tiles (like grass).
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * LaserLength, Projectile.width + 16, DelegateMethods.CutTiles);
        }
    }
}