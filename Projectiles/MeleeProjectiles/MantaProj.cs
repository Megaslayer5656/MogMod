using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class MantaProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public int HomingCooldown = 0;
        public override void SetStaticDefaults()
        {
            // anti homing
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;

            // cool after effect
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 52;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            // slow down the more hits it did
            Projectile.rotation += MathHelper.ToRadians(30f) / (float)Math.Log(6f - Projectile.penetrate + 2f) / 1.4f;

            // wait a while before homing after hitting an enemy
            if (HomingCooldown > 0)
            {
                HomingCooldown--;
            }
            else
            {
                MogModUtils.HomeInOnNPC(Projectile, true, 950f, 10f, 25f);
            }
        }
        public override bool? CanHitNPC(NPC target) => HomingCooldown > 0 ? false : (bool?)null;

        public override bool CanHitPvp(Player target) => HomingCooldown <= 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HomingCooldown = 30;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            HomingCooldown = 30;
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 48;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            SoundEngine.PlaySound(SoundID.NPCDeath39, Projectile.position);
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ShimmerSpark, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int i = 0; i < 10; i++)
            {
                int dusty = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ShimmerSpark, 0f, 0f, 100, default, 3f);
                Main.dust[dusty].noGravity = true;
                Main.dust[dusty].velocity *= 5f;
                dusty = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ShimmerSpark, 0f, 0f, 100, default, 2f);
                Main.dust[dusty].velocity *= 2f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}