using Microsoft.Xna.Framework;
using MogMod.Common.Graphics;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class KaminariZipProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public Player Owner => Main.player[Projectile.owner];
        public ref float Timer => ref Projectile.ai[0];
        public int DrawTime = 5; // time until draw dusts
        public int ShootCooldown = 60; // time until can shoot
        public bool Initialized = false; // used instead of onspawn since onspawn doesn't sync in multiplayer
        public int NumAnimationFrames = 4; // total sprites in spritesheet
        public int AnimationFrameTime = 5; // number of frames before updating sprite
        public static Color Colour => new(126, 233, 254);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // set to 2 so we can draw trails
            Main.projFrames[Projectile.type] = NumAnimationFrames;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 46;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.hide = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            MogPlayer mogPlayer = Owner.MogMod();
            if (!Initialized) // if not initialized, initialize and set timer to shootcooldown
            {
                Timer = ShootCooldown;
                Initialized = true;
            }
            Projectile.frameCounter++; // update frameCounter
            if (Projectile.frameCounter % (AnimationFrameTime) == 0) // when frameCounter % animationframetime == 0, update proj frame
                Projectile.frame = Projectile.frame >= NumAnimationFrames - 1 ? 0 : Projectile.frame + 1;
            if (!mogPlayer.kaminariActive || !Projectile.active || Owner.dead)
            {
                Timer = ShootCooldown;
                Projectile.active = false;
                //Main.NewText($"killing proj", Color.Red);
                Projectile.Kill(); // kill the projectile if not zipping
            }
            else
            {
                Timer--;
                Projectile.timeLeft = 5; // constantly refresh timeLeft so it doesn't die

                Projectile.rotation = Projectile.velocity.ToRotation(); // rotating the projectile on velocity is important if we want to draw trails
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 playerPosition = Owner.Center + Vector2.UnitY * Owner.gfxOffY; // get the players position
                    Vector2 orbAttemptedVelocity = Vector2.Zero.MoveTowards(playerPosition - Projectile.Center, 9999f);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, orbAttemptedVelocity, 1f); // set the projectiles velocity towards the player
                    Projectile.netUpdate = true; // sync in multiplayer
                }
                if (Timer <= ShootCooldown - DrawTime)
                {
                    // dust and lighting effects
                    Lighting.AddLight(Projectile.Center, Colour.ToVector3() * (Projectile.scale * 0.5f));
                    for (int i = 0; i < 2; i++)
                    {
                        float shortXVel = Projectile.velocity.X / 3f * (float)i;
                        float shortYVel = Projectile.velocity.Y / 3f * (float)i;
                        int fourConst = 4;
                        int fireDust = Dust.NewDust(new Vector2(Projectile.position.X + (float)fourConst, Projectile.position.Y + (float)fourConst), Projectile.width - fourConst * 2, Projectile.height - fourConst * 2, DustID.Electric, 0f, 0f, 100, default, 1.2f);
                        Dust dust = Main.dust[fireDust];
                        dust.noGravity = true;
                        dust.velocity *= 0.1f;
                        dust.velocity += Projectile.velocity * 0.1f;
                        dust.position.X -= shortXVel;
                        dust.position.Y -= shortYVel;
                    }
                    if (Main.rand.NextBool(10))
                    {
                        int otherFourConst = 4;
                        int fireDustSmol = Dust.NewDust(new Vector2(Projectile.position.X + (float)otherFourConst, Projectile.position.Y + (float)otherFourConst), Projectile.width - otherFourConst * 2, Projectile.height - otherFourConst * 2, DustID.FireworksRGB, 0f, 0f, 100, default, 0.6f);
                        Main.dust[fireDustSmol].velocity *= 0.25f;
                        Main.dust[fireDustSmol].velocity += Projectile.velocity * 0.5f;
                        Main.dust[fireDustSmol].color = Color.Lerp(Colour, Color.Turquoise, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f);
                    }

                    // if time is greater than shootCooldown, shoot proj, then apply shootCooldown to timer
                    if (Timer <= 0)
                    {
                        NPC target = Projectile.Center.ClosestNPCAt(1000);
                        MogModUtils.MagnetSphereHitscan(Projectile, Vector2.Distance(Projectile.Center, target.Center), 8f, 0, 3, ModContent.ProjectileType<OrchidBeam>(), 1D, true);
                        Timer = ShootCooldown / (KeybindSystem.ArmorSetBonusKeybind.Current ? 6 : 3); // set timer to be a third of shootCooldown so that holding the zip is incentivized
                    }
                }
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);
        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawer trailDrawer = default;
            string type = "MagicMissile";
            Color innerColor = Colour;
            Color outerColor = Color.Turquoise;
            float width = 0.6f;
            float length = 30f;
            trailDrawer.Draw(Projectile, type, outerColor, innerColor, width, length, length + 8f);
            return true;
        }
    }
}