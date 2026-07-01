﻿using Microsoft.Xna.Framework;
using MogMod.Projectiles.Classless;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class HellfireCannonProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public ref float Time => ref Projectile.ai[0];
        public bool CanExplode = true;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 600;
        }
        public override void AI()
        {
            Time++;
            if (Time < 3)
                Projectile.tileCollide = false;
            else
                Projectile.tileCollide = true;
            float rotateratio = 0.019f;
            float rotation = (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * rotateratio;
            Projectile.rotation += rotation * Projectile.direction;
            Projectile.velocity.Y = Projectile.velocity.Y + 0.25f;
            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;
            if (Time >= 8f)
            {
                float flameDustSize = Utils.GetLerpValue(6f, 12f, Time, true);
                Dust flameDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Flare, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 10, default, 0.75f);
                if (Main.rand.NextBool(3))
                {
                    flameDust.scale *= 3f;
                    flameDust.velocity *= 1.5f;
                }
                flameDust.noGravity = true;
                flameDust.scale *= flameDustSize * 0.8f;
                flameDust.velocity += Projectile.velocity;
                int fireDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Flare, 0f, 0f, 200, default, 2.7f);
                Dust dust = Main.dust[fireDust];
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire3, 180);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.OnFire3, 180);
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item62, Projectile.position);
            for (int k = 0; k < 15; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Flare, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            for (int i = 0; i < 9; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Smoke, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, 100, default, 1.7f);
                Main.dust[dust].velocity *= 1.4f;
            }
            for (int n = 0; n < 6; n++)
            {
                float swirlRotation = Main.GlobalTimeWrappedHourly * -5.75f + (MathHelper.TwoPi / 6f * n);
                Vector2 swirlPos = Projectile.Center + Vector2.UnitX.RotatedBy(swirlRotation) * 20f;
                Vector2 swirlVelocity = Vector2.Normalize(swirlPos - Projectile.Center).RotatedBy(MathHelper.ToRadians(20)) * 2f;
                Dust swirlDust = Dust.NewDustPerfect(swirlPos, DustID.CopperCoin, swirlVelocity * Main.rand.NextFloat(5f, 7f), 0, default, 1.5f);
                swirlDust.noGravity = true;
                swirlDust.fadeIn = .6f;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                var source = Projectile.GetSource_FromThis();
                Projectile explosion = Projectile.NewProjectileDirect(source, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HellfireBoom>(), Projectile.damage * 2, 0f, Main.myPlayer, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                explosion.DamageType = DamageClass.Ranged;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}