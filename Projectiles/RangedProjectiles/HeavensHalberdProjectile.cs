using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using MogMod.Utilities;
using MonoMod.Core.Utils;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class HeavensHalberdProjectile : ModProjectile
    {
        private bool initialized = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            DrawOffsetX = -10;
            DrawOriginOffsetY = 0;
            DrawOriginOffsetX = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            Projectile.extraUpdates = 2;
            AIType = ProjectileID.JavelinFriendly;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            //Code to make it not shoot backwards
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            //Gravity
            Projectile.velocity.Y = Projectile.velocity.Y + 0.020f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }

            //Dust trail
            if (Main.rand.NextBool(25))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 1.2f);
                Main.dust[d].position = Projectile.Center;
                Main.dust[d].noLight = true;
            }
        }

        // makes it summon an additional projectile
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            //Dust splash
            int dustsplash = 0;
            while (dustsplash < 4)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 0.9f);
                Main.dust[d].position = Projectile.Center;
                dustsplash += 1;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!initialized)
            {
                initialized = true;
                if (Projectile.owner == Main.myPlayer)
                {
                    SummonLasers();
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!initialized)
            {
                initialized = true;
                if (Projectile.owner == Main.myPlayer)
                {
                    SummonLasers();
                }
            }
        }

        // blurs the projectile
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        private void SummonLasers()
        {
            var source = Projectile.GetSource_FromThis();
            SoundEngine.PlaySound(SoundID.NPCDeath55, Projectile.Center);
            for (int n = 0; n < 4; n++)
            {
                MogModUtils.ProjectileRain(source, Projectile.Center, 100f, 50f, 1500f, 1500f, 10f, ModContent.ProjectileType<HeavensHalberdProj>(), Convert.ToInt32(Projectile.damage * .55), Projectile.knockBack, Projectile.owner);
            }
            Projectile.ExpandHitboxBy(18);
            int dustAmt = 18;
            for (int j = 0; j < dustAmt; j++)
            {
                Vector2 dustRotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                dustRotate = dustRotate.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustDirection = dustRotate - Projectile.Center;
                int killDust = Dust.NewDust(dustRotate + dustDirection, 0, 0, DustID.YellowStarDust, dustDirection.X, dustDirection.Y, 100, default, 1.2f);
                Main.dust[killDust].noGravity = true;
                Main.dust[killDust].noLight = true;
                Main.dust[killDust].velocity = dustDirection;
            }
            Projectile.ExpandHitboxBy(18);
            for (int j = 0; j < dustAmt; j++)
            {
                Vector2 dustRotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 1f; //0.75
                dustRotate = dustRotate.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustDirection = dustRotate - Projectile.Center;
                int killDust = Dust.NewDust(dustRotate + dustDirection, 0, 0, DustID.ShimmerSpark, dustDirection.X, dustDirection.Y, 100, default, 1.2f);
                Main.dust[killDust].noGravity = true;
                Main.dust[killDust].noLight = true;
                Main.dust[killDust].velocity = dustDirection;
            }
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;
            Projectile.Damage();
        }
    }
}