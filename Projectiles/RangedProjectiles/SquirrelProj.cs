﻿using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class SquirrelProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {
            Projectile.frame = (int)Projectile.ai[0];
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] == 1f)
                MogModUtils.HomeInOnNPC(Projectile, false, 1000f, 12f, 15f);
            if (Main.rand.NextBool(5))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Projectile.ai[0] == 1f ? DustID.PinkTorch : DustID.DirtSpray, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.9f);
                Main.dust[d].position = Projectile.Center;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 1f)
            {
                if (Projectile.ai[1] < 30f)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit1, Projectile.position);
                    if (Projectile.velocity.X != oldVelocity.X)
                        Projectile.velocity.X = -oldVelocity.X;
                    if (Projectile.velocity.Y != oldVelocity.Y)
                        Projectile.velocity.Y = -oldVelocity.Y;
                    Projectile.ai[1] += 1f;
                }
                else
                    if (Projectile.velocity.X != oldVelocity.X || Projectile.velocity.Y != oldVelocity.Y)
                        Projectile.Kill();
                return false;
            }
            else
                return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Confused, Projectile.ai[0] == 1f ? 300 : 0);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Confused, Projectile.ai[0] == 1f ? 300 : 0);
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
            for (int k = 0; k < 15; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, Projectile.ai[0] == 1f ? DustID.PurpleTorch : DustID.DirtSpray, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            for (int i = 0; i < 9; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, Projectile.ai[0] == 1f ? DustID.PinkTorch : DustID.Dirt, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, 100, default, 1.7f);
                Main.dust[dust].velocity *= 1.4f;
            }
            if (Projectile.ai[0] == 1f)
                for (int n = 0; n < 6; n++)
                {
                    float swirlRotation = Main.GlobalTimeWrappedHourly * -5.75f + (MathHelper.TwoPi / 6f * n);
                    Vector2 swirlPos = Projectile.Center + Vector2.UnitX.RotatedBy(swirlRotation) * 20f;
                    Vector2 swirlVelocity = Vector2.Normalize(swirlPos - Projectile.Center).RotatedBy(MathHelper.ToRadians(20)) * 2f;
                    Dust swirlDust = Dust.NewDustPerfect(swirlPos, DustID.PlatinumCoin, swirlVelocity * Main.rand.NextFloat(5f, 7f), 0, default, 1.5f);
                    swirlDust.noGravity = true;
                    swirlDust.fadeIn = .6f;
                }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}