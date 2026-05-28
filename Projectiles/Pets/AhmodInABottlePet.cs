using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Pets
{
    // code taken from calamity mod scal pet
    public class AhmodInABottlePet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Pets";
        public override string Texture => "MogMod/NPCs/Enemies/Ahmod";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Type], 4)
            .WithOffset(-12f, -18).WithSpriteDirection(-1).WhenNotSelected(0, 0);
        }
        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            MogPlayer modPlayer = player.MogMod();
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }
            if (player.dead)
                modPlayer.ahmodPet = false;
            if (modPlayer.ahmodPet)
                Projectile.timeLeft = 2;
            if (Projectile.velocity.X < 0f)
                Projectile.direction = 1;
            else
                Projectile.direction = -1;
            Projectile.spriteDirection = Projectile.direction;
            float passiveMvtFloat = 0.5f;
            Projectile.tileCollide = false;
            int range = 300;
            Vector2 center = Projectile.Center;
            float distX = player.Center.X - center.X;
            float distY = player.Center.Y - center.Y;
            float playerDist = player.Distance(center);
            float returnSpeed = 18f;
            float maxDist = 2000f;
            bool tooFar = playerDist > maxDist;
            if (playerDist < (float)range && Main.player[Projectile.owner].velocity.Y == 0f && Projectile.position.Y + (float)Projectile.height <= Main.player[Projectile.owner].position.Y + (float)Main.player[Projectile.owner].height && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                if (Projectile.velocity.Y < -6f)
                    Projectile.velocity.Y = -6f;
            }
            if (playerDist < 150f)
            {
                if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                    Projectile.velocity *= 0.99f;
                passiveMvtFloat = 0.01f;
                if (distX < -2f)
                    distX = -2f;
                if (distX > 2f)
                    distX = 2f;
                if (distY < -2f)
                    distY = -2f;
                if (distY > 2f)
                    distY = 2f;
            }
            else
            {
                if (playerDist > 300f)
                    passiveMvtFloat = 0.2f;
                playerDist = returnSpeed / playerDist;
                distX *= playerDist;
                distY *= playerDist;
            }
            if (tooFar)
            {
                Projectile.Center = Main.player[Projectile.owner].Center;
                Projectile.velocity = Vector2.Zero;
                if (Main.myPlayer == Projectile.owner)
                    Projectile.netUpdate = true;
            }
            if (Math.Abs(distX) > Math.Abs(distY) || passiveMvtFloat == 0.05f)
            {
                if (Projectile.velocity.X < distX)
                {
                    Projectile.velocity.X += passiveMvtFloat;
                    if (passiveMvtFloat > 0.05f && Projectile.velocity.X < 0f)
                        Projectile.velocity.X += passiveMvtFloat;
                }
                if (Projectile.velocity.X > distX)
                {
                    Projectile.velocity.X -= passiveMvtFloat;
                    if (passiveMvtFloat > 0.05f && Projectile.velocity.X > 0f)
                        Projectile.velocity.X -= passiveMvtFloat;
                }
            }
            if (Math.Abs(distX) <= Math.Abs(distY) || passiveMvtFloat == 0.05f)
            {
                if (Projectile.velocity.Y < distY)
                {
                    Projectile.velocity.Y += passiveMvtFloat;
                    if (passiveMvtFloat > 0.05f && Projectile.velocity.Y < 0f)
                        Projectile.velocity.Y += passiveMvtFloat;
                }
                if (Projectile.velocity.Y > distY)
                {
                    Projectile.velocity.Y -= passiveMvtFloat;
                    if (passiveMvtFloat > 0.05f && Projectile.velocity.Y > 0f)
                        Projectile.velocity.Y -= passiveMvtFloat;
                }
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
                Projectile.frameCounter = 0;
            }
        }
    }
}