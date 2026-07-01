using Microsoft.Xna.Framework;
﻿using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Classless
{
    public class HellfireExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Classless";
        public override void SetStaticDefaults() => Main.projFrames[Type] = 5;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.9f, 0.8f, 0.6f);
            Projectile.ai[1] += 0.01f;
            Projectile.scale = Projectile.ai[1] * 0.8f;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= (float)(3 * Main.projFrames[Type]))
            {
                Projectile.Kill();
                return;
            }
            int incrementer = Projectile.frameCounter + 1;
            Projectile.frameCounter = incrementer;
            if (incrementer >= 3)
            {
                Projectile.frameCounter = 0;
                incrementer = Projectile.frame + 1;
                Projectile.frame = incrementer;
                if (incrementer >= Main.projFrames[Type])
                {
                    Projectile.hide = true;
                }
            }
            Projectile.alpha -= 63;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            if (Projectile.ai[0] == 1f)
            {
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = (int)(52f * Projectile.scale);
                Projectile.Center = Projectile.position;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                for (int dustIndexA = 0; dustIndexA < 4; dustIndexA = incrementer + 1)
                {
                    int smoky = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                    Main.dust[smoky].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                    incrementer = dustIndexA;
                }
                for (int dustIndexB = 0; dustIndexB < 10; dustIndexB = incrementer + 1)
                {
                    int fireDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 200, default, 2.7f);
                    Dust dust = Main.dust[fireDust];
                    dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                    fireDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                    dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                    dust.velocity *= 2f;
                    dust.noGravity = true;
                    dust.fadeIn = 2.5f;
                    incrementer = dustIndexB;
                }
                for (int dustIndexC = 0; dustIndexC < 5; dustIndexC = incrementer + 1)
                {
                    int fireDust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default, 2.7f);
                    Dust dust = Main.dust[fireDust2];
                    dust.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 2f;
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                    incrementer = dustIndexC;
                }
                for (int dustIndexD = 0; dustIndexD < 10; dustIndexD = incrementer + 1)
                {
                    int smokier = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 0, default, 1.5f);
                    Dust dust = Main.dust[smokier];
                    dust.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 2f;
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                    incrementer = dustIndexD;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor) => Projectile.ai[0] > 1f;
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 127);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.direction = Main.player[Projectile.owner].direction;
            target.AddBuff(BuffID.OnFire3, 300);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)   
        {
            Projectile.direction = Main.player[Projectile.owner].direction;
            target.AddBuff(BuffID.OnFire3, 300);
        }
    }
}