using Microsoft.Xna.Framework;
using MogMod.Common.Classes;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class RoilingMagmaProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public ref float Time => ref Projectile.ai[0];
        public ref float UndergroundTime => ref Projectile.ai[1];
        public bool inGround = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.hide = true;
        }
        public override void AI()
        {
            Time++;
            float rotateratio = 0.019f;
            float rotation = (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * rotateratio;
            Projectile.rotation += rotation * Projectile.direction;
            Projectile.velocity.Y = Projectile.velocity.Y + 0.25f;
            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;
            if (Time >= 8f)
            {
                float flameDustSize = Utils.GetLerpValue(6f, 12f, Time, true);
                Dust flameDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? DustID.Flare : DustID.Lava, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 10, default, 0.75f);
                if (Main.rand.NextBool(3))
                {
                    flameDust.scale *= 3f;
                    flameDust.velocity *= 1.5f;
                }
                flameDust.noGravity = true;
                flameDust.scale *= flameDustSize * 0.8f;
                flameDust.velocity += Projectile.velocity;
                int fireDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 25, 0f, 0f, 200, default, 0.7f);
                Dust dust = Main.dust[fireDust];
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)Projectile.width / 4f;
                dust.noGravity = true;
                dust.velocity *= 1.2f;
            }
            if (inGround)
                UndergroundTime++;
            if (UndergroundTime >= 3 && Projectile.tileCollide == false)
            {
                Projectile.tileCollide = true;
                Projectile.velocity *= 0.8f;
                Projectile.velocity.X = 0f;
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity *= 0.6f;
            target.AddBuff(BuffID.OnFire, 180);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.velocity *= 0.6f;
            target.AddBuff(BuffID.OnFire, 180);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!inGround)
            {
                inGround = true;
                Projectile.tileCollide = false;
                Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            }
            if (UndergroundTime >= 60)
                return true;
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item62, Projectile.position);
            for (int k = 0; k < 21; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, Main.rand.NextBool() ? DustID.Flare : DustID.Lava, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.8f);
            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 25, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, 100, default, 1.7f);
                Main.dust[dust].velocity *= 1.4f;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                var source = Projectile.GetSource_FromThis();
                int proj = Main.zenithWorld ? Main.rand.NextBool(10) ? Main.rand.Next(50, 100) : Main.rand.Next(3, 6) : Main.rand.Next(3, 6);
                for (int n = 0; n < proj; n++)
                {
                    float Spread = Main.rand.NextFloat(0.2f, 0.6f);
                    Vector2 kirk = new(0, -Main.rand.Next(7, 15));
                    Vector2 velocity = kirk.RotatedByRandom(Spread);
                    int type = (Main.zenithWorld && Projectile.ai[2] != 1f) ? Projectile.type : ModContent.ProjectileType<RoilingMagmaShard>();
                    Projectile.NewProjectile(source, Projectile.Center, velocity, type, Main.zenithWorld ? (int)(Projectile.damage * 1.2f) : (int)(Projectile.damage * 0.8f), Projectile.knockBack, Main.myPlayer, ai2: Main.zenithWorld ? 1f : 0f);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}