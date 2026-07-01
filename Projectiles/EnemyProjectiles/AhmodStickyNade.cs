using Microsoft.Xna.Framework;
using MogMod.Projectiles.Melee;
using MogMod.Utilities;
﻿using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.EnemyProjectiles
{
    public class AhmodStickyNade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.EnemyProjectiles";
        public bool exploding = false;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Generic;
        }
        public override void AI()
        {
            if (Main.rand.NextBool(12))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.t_Slime, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, Color.LightBlue);
            //Sticky Behaviour
            if (Projectile.ai[0] != 1f)
            {
                Projectile.StickToTiles(true, false);
                Projectile.localAI[1] += 1f;
                if (Projectile.localAI[1] > 10f)
                {
                    Projectile.localAI[1] = 10f;
                    if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                    {
                        Projectile.velocity.X *= 0.97f;
                        if (Math.Abs(Projectile.velocity.X) < 0.01f)
                        {
                            Projectile.velocity.X = 0f;
                            Projectile.netUpdate = true;
                        }
                    }
                    Projectile.velocity.Y += 0.2f;
                }
                Projectile.rotation += Projectile.velocity.X * 0.1f;
            }
            if (Projectile.timeLeft <= 60)
            {
                exploding = true;
                int size = 30;
                int dust3 = Dust.NewDust(Projectile.Center, (int)(size / 2), (int)(size / 2), DustID.Smoke, 0f, 0f, 100, default, 1.7f);
                Main.dust[dust3].velocity *= 1.4f;
                for (int i = 0; i < 2; i++)
                {
                    int dust2 = Dust.NewDust(Projectile.Center, (int)(size / 2), (int)(size / 2), DustID.Torch, 0f, 0f, 100, default, 2.4f);
                    Main.dust[dust2].noGravity = true;
                    Main.dust[dust2].velocity *= 5f;
                    dust2 = Dust.NewDust(Projectile.Center, (int)(size / 2), (int)(size / 2), DustID.Torch, 0f, 0f, 100, default, 1.6f);
                    Main.dust[dust2].velocity *= 3f;
                }
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!exploding)
                target.AddBuff(BuffID.OgreSpit, 60);
            else
                target.AddBuff(BuffID.OnFire, 120);
            if (Projectile.timeLeft > 60)
                Projectile.timeLeft = 60;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (!exploding)
            {
                modifiers.FinalDamage *= 0f;
                modifiers.Knockback *= 0f;
            }
        }
        public override bool? CanDamage()
        {
            if (exploding && Projectile.timeLeft > 0)
                return false;
            else
                return true;
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);
            int size = 30;
            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, size, size, DustID.t_Slime, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 0, Color.Blue, Main.rand.NextFloat(.8f, 1.6f));
                Main.dust[dust].velocity *= 1.4f;
            }
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, size, size, DustID.Smoke, 0f, 0f, 100, default, 1.2f);
                Main.dust[dust].velocity *= 1.4f;
            }
            for (int i = 0; i < 9; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, size, size, DustID.Torch, 0f, 0f, 100, default, 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 5f;
                dust = Dust.NewDust(Projectile.Center, size, size, DustID.Torch, 0f, 0f, 100, default, 1.1f);
                Main.dust[dust].velocity *= 3f;
            }
            Projectile.localAI[1] = -1f;
            Projectile.maxPenetrate = -1;
            Projectile.Damage();
        }
    }
}