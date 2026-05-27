﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    // code taken from briny baron projectiles from calamity mod
    public class FlameTornadoProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override void SetStaticDefaults() => Main.projFrames[Type] = 6;
        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }
        public override void AI()
        {
            int projScale = 22;
            float scaleModifier = 2.5f;
            int projWidth = 150;
            int projHeight = 42;
            if (Projectile.velocity.X != 0f)
                Projectile.direction = Projectile.spriteDirection = -Math.Sign(Projectile.velocity.X);
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 2)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 6)
                Projectile.frame = 0;
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
                Projectile.scale = ((float)(projScale) - Projectile.ai[1]) * scaleModifier / (float)(projScale);
                Projectile.width = (int)((float)projWidth * Projectile.scale);
                Projectile.height = (int)((float)projHeight * Projectile.scale);
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[1] != -1f)
            {
                Projectile.scale = ((float)(projScale) - Projectile.ai[1]) * scaleModifier / (float)(projScale);
                Projectile.width = (int)((float)projWidth * Projectile.scale);
                Projectile.height = (int)((float)projHeight * Projectile.scale);
            }
            if (!Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.alpha -= 3;
                if (Projectile.alpha < 60)
                    Projectile.alpha = 60;
            }
            else
            {
                Projectile.alpha += 3;
                if (Projectile.alpha > 150)
                    Projectile.alpha = 150;
            }
            if (Projectile.ai[0] > 0f)
                Projectile.ai[0] -= 1f;
            if (Projectile.ai[0] == 1f && Projectile.ai[1] > 0f && Projectile.owner == Main.myPlayer)
            {
                Projectile.netUpdate = true;
                Vector2 center = Projectile.Center;
                center.Y -= (float)projHeight * Projectile.scale / 2f;
                float nextSegment = ((float)(projScale) - Projectile.ai[1] + 1f) * scaleModifier / (float)(projScale);
                center.Y -= (float)projHeight * nextSegment / 2f;
                center.Y += 2f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), center.X, center.Y, Projectile.velocity.X, Projectile.velocity.Y, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, 8f, Projectile.ai[1] - 1f);
            }
            if (Projectile.ai[0] <= 0f)
            {
                float swaySize = MathHelper.Pi / 30f;
                float smolWidth = (float)Projectile.width / 5f;
                smolWidth *= 2f;
                float projXChange = (float)(Math.Cos((double)(swaySize * -(double)Projectile.ai[0])) - 0.5) * smolWidth;
                Projectile.position.X = Projectile.position.X - projXChange * (float)-(float)Projectile.direction;
                Projectile.ai[0] -= 1f;
                projXChange = (float)(Math.Cos((double)(swaySize * -(double)Projectile.ai[0])) - 0.5) * smolWidth;
                Projectile.position.X = Projectile.position.X + projXChange * (float)-(float)Projectile.direction;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<InfernoDebuff>(), 180);
        public override Color? GetAlpha(Color lightColor) => new Color(255, 53, 53, Projectile.alpha);
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            int framing = Terraria.GameContent.TextureAssets.Projectile[Type].Value.Height / Main.projFrames[Type];
            int y6 = framing * Projectile.frame;
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, framing)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)framing / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}