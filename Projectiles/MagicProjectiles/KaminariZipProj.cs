using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Graphics;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Projectiles.Classless;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class KaminariZipProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public Player Owner => Main.player[Projectile.owner];
        public ref float Timer => ref Projectile.ai[0];
        public bool kaminari = false;
        public bool Initialized = false; // used instead of onspawn since onspawn doesn't sync in multiplayer
        public int NumAnimationFrames = 4; // total sprites in spritesheet
        public int AnimationFrameTime = 5; // number of frames before updating sprite
        public int Size = 46;
        public static Color Colour => new(126, 233, 254);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 80;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // set to 2 so we can draw trails
            Main.projFrames[Projectile.type] = NumAnimationFrames;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = Size;

            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hide = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            MogPlayer mogPlayer = Owner.MogMod();
            kaminari = mogPlayer.wearingKaminari && mogPlayer.kaminariActive;

            if (!Initialized) // if not initialized, initialize and set timer to shootcooldown
            {
                int dustTimer = 3;
                while (dustTimer >= 0)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? DustID.FireworksRGB : DustID.Electric, Main.rand.NextVector2Circular(Projectile.width * 0.5f, Projectile.height * 0.5f), 100, Colour, 1f);
                        dust.noGravity = true;
                        dust.velocity *= 1.2f;
                        dust.scale *= 1.15f;
                    }
                    dustTimer--;
                }
                Initialized = true;
            }
            Projectile.frameCounter++; // update frameCounter
            if (Projectile.frameCounter % (AnimationFrameTime) == 0) // when frameCounter % animationframetime == 0, update proj frame
                Projectile.frame = Projectile.frame >= NumAnimationFrames - 1 ? 0 : Projectile.frame + 1;

            int accCheck = kaminari ? 1 : 0;

            if (accCheck != 0 && Projectile.active)
            {
                Projectile.position = Projectile.Center;
                Projectile.Center = Projectile.position;
                Projectile.rotation = Projectile.velocity.ToRotation(); // rotating the projectile on velocity is important if we want to draw trails
                Timer++;
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 playerPosition = Owner.Center + Vector2.UnitY * Owner.gfxOffY; // get the players position
                    Vector2 orbAttemptedVelocity = Vector2.Zero.MoveTowards(playerPosition - Projectile.Center, 9999f);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, orbAttemptedVelocity, 1f); // set the projectiles velocity towards the player

                    var source = Projectile.GetSource_FromThis();
                    var type = ModContent.ProjectileType<TrailProj>();
                    var p = Projectile.NewProjectileDirect(source, Owner.Center, Vector2.Zero, type, Projectile.damage, 0f, Projectile.owner);
                    p.DamageType = DamageClass.Magic;
                    p.timeLeft = 70;
                    //p.ai[2] = 6f; // used to see the proj


                    NPC target = Projectile.Center.ClosestNPCAt(1000);
                    if (target != null && Timer % (mogPlayer.kaminariCooldownMax / (KeybindSystem.ZipSlowdownKeybind.Current ? 2 : 4)) == 0) MogModUtils.MagnetSphereHitscan(Projectile, Vector2.Distance(Projectile.Center, target.Center), 8f, 0, 3, ModContent.ProjectileType<KaminariZipBeam>(), 1D, true, ai1: Main.rand.Next(0,2));

                    Projectile.netUpdate = true; // sync in multiplayer
                }

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
                Projectile.timeLeft = 2; // constantly refresh timeLeft so it doesn't die
            }

            if (Owner.dead || accCheck == 0) Projectile.Kill(); // if the player's dead, delete the projectile.
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int size = (Size * 2);
            hitbox.Inflate(size, size);
        }
        public override void OnKill(int timeLeft)
        {
            int dustTimer = 3;
            while (dustTimer >= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? DustID.FireworksRGB : DustID.Electric, Main.rand.NextVector2Circular(-Projectile.width * 0.5f, -Projectile.height * 0.5f), 100, Colour, 1f);
                    dust.noGravity = true;
                    dust.velocity *= 1.2f;
                    dust.scale *= 1.15f;
                }
                dustTimer--;
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
            trailDrawer.Draw(Projectile, type, outerColor, innerColor, width, length, length + 30f);

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 scale = Vector2.One * Projectile.scale * 2f;
            Vector2 newScale = scale + scale * (float)Math.Cos(Main.GlobalTimeWrappedHourly * ((float)Math.PI * 1f)) * 0.2f;
            Main.EntitySpriteDraw(texture, drawPosition, sourceRectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, sourceRectangle.Size() * 0.5f, newScale, SpriteEffects.None);

            // draw glow effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/CircleGradient").Value;
            for (int i = 0; i < 4; i++)
            {
                Color drawColour = Projectile.GetAlpha(Colour);
                Vector2 glowScale = scale + scale * (float)Math.Cos(Main.GlobalTimeWrappedHourly * ((float)Math.PI * 8f)) * 0.2f;
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, drawColour * 0.85f, Projectile.rotation, bloomTex.Size() * 0.5f, glowScale * (0.2f * i), SpriteEffects.None);
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, drawColour * 0.1f, Projectile.rotation, bloomTex.Size() * 0.5f, glowScale * (0.3f * i), SpriteEffects.None);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}