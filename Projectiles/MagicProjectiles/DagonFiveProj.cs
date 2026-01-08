using System;
using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class DagonFiveProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        Random random = new Random();
        private bool initialized = false;
        public int randNumProjectiles;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (!initialized)
            {
                SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                initialized = true;
                float dustAmt = 16f;
                int d = 0;
                while ((float)d < dustAmt)
                {
                    Vector2 offset = Vector2.UnitX * 0f;
                    offset += -Vector2.UnitY.RotatedBy((double)((float)d * (MathHelper.TwoPi / dustAmt)), default) * new Vector2(1f, 4f);
                    offset = offset.RotatedBy((double)Projectile.velocity.ToRotation(), default);
                    int i = Dust.NewDust(Projectile.Center, 0, 0, DustID.RedTorch, 0f, 0f, 0, default, 1f);
                    Main.dust[i].scale = 1.5f;
                    Main.dust[i].noGravity = true;
                    Main.dust[i].position = Projectile.Center + offset;
                    Main.dust[i].velocity = Projectile.velocity * 0f + offset.SafeNormalize(Vector2.UnitY) * 1f;
                    d++;
                }
            }

            float pi = MathHelper.Pi;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] == 48f)
            {
                Projectile.ai[0] = 0f;
            }
            else
            {
                for (int d = 0; d < 2; d++)
                {
                    Vector2 offset = Vector2.UnitX * -12f;
                    offset = -Vector2.UnitY.RotatedBy((double)(Projectile.ai[0] * pi / 24f + (float)d * pi), default) * new Vector2(5f, 10f) - Projectile.rotation.ToRotationVector2() * 10f;
                    int i = Dust.NewDust(Projectile.Center, 0, 0, DustID.RedTorch, 0f, 0f, 160, default, 1f);
                    Main.dust[i].scale = 0.75f;
                    Main.dust[i].noGravity = true;
                    Main.dust[i].position = Projectile.Center + offset;
                    Main.dust[i].velocity = Projectile.velocity;
                }
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 0f)
            {
                for (int d = 0; d < 2; d++)
                {
                    Vector2 source = Projectile.position;
                    source -= Projectile.velocity * ((float)d * 0.25f);
                    int i = Dust.NewDust(source, 1, 1, 66, 0f, 0f, 0, Color.DarkRed, 1f);
                    Main.dust[i].noGravity = true;
                    Main.dust[i].position = source;
                    Main.dust[i].scale = Main.rand.NextFloat(0.91f, 1.417f);
                    Main.dust[i].velocity *= 0.1f;
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                SummonLasers();
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                SummonLasers();
            }
            target.AddBuff(BuffID.Daybreak, 600);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                SummonLasers();
            }
            target.AddBuff(BuffID.Daybreak, 600);
        }

        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            int dustAmt = Main.rand.Next(4, 10);
            for (int d = 0; d < dustAmt; d++)
            {
                int fire = Dust.NewDust(Projectile.Center, 0, 0, DustID.RedTorch, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[fire];
                dust.velocity *= 1.1f;
                dust.velocity.Y -= 1f;
                dust.velocity += -Projectile.velocity * (Main.rand.NextFloat() * 2f - 1f) * 0.5f;
                dust.scale = 1f;
                dust.fadeIn = 2f;
                dust.noGravity = true;
            }
        }
        // make it summon either giant homing orb or lots of bolts using switch and case slop
        private void SummonLasers()
        {
            var source = Projectile.GetSource_FromThis();
            switch (Projectile.ai[1])
            {
                case 0f:
                    Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DagonFiveExplosion>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                    break;
                case 1f:
                    SoundEngine.PlaySound(SoundID.NPCDeath55, Projectile.Center);
                    for (int n = 0; n < 4; n++)
                    {
                        MogModUtils.ProjectileRain(source, Projectile.Center, 100f, 50f, 1500f, 1500f, 10f, ModContent.ProjectileType<DagonFiveSkyProj>(), Convert.ToInt32(Projectile.damage * .55), Projectile.knockBack, Projectile.owner);
                    }
                    break;
                case 2f:
                    float spread = 30f * 0.01f * MathHelper.PiOver2;
                    double startAngle = Math.Atan2(Projectile.velocity.X, Projectile.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    for (int i = 0; i < 2; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 8f * i;
                        Projectile.NewProjectile(source, Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<DagonFiveOrb>(), Convert.ToInt32(Projectile.damage * .8), Projectile.knockBack, Projectile.owner);
                        Projectile.NewProjectile(source, Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<DagonFiveOrb>(), Convert.ToInt32(Projectile.damage * .8), Projectile.knockBack, Projectile.owner);
                    }
                    break;
            }
        }
    }
}