using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class MeteorHammerProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 240;
            Projectile.height = 240;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (Projectile.position.Y > Main.player[Projectile.owner].position.Y - 300f)
            {
                Projectile.tileCollide = true;
            }
            if ((double)Projectile.position.Y < Main.worldSurface * 16.0)
            {
                Projectile.tileCollide = true;
            }
            Projectile.scale = Projectile.ai[1] + 7f;
            Projectile.rotation += Projectile.velocity.X * 0.2f;
            Vector2 position = Projectile.Center + Vector2.Normalize(Projectile.velocity) * (Projectile.scale * 16f);
            Dust blueDust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MoonBoulder, 0f, 0f, 0, default, 2f)];
            blueDust.position = position;
            blueDust.velocity = Projectile.velocity.RotatedBy(1.5707963705062866, default) / 0.4f + Projectile.velocity / 2f;
            blueDust.position += Projectile.velocity.RotatedBy(1.5707963705062866, default) * 8f;
            blueDust.fadeIn = 0.5f;
            blueDust.noGravity = true;
            blueDust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MoonBoulder, 0f, 0f, 0, default, 2f)];
            blueDust.position = position;
            blueDust.velocity = Projectile.velocity.RotatedBy(-1.5707963705062866, default) / 0.4f + Projectile.velocity / 2f;
            blueDust.position += Projectile.velocity.RotatedBy(-1.5707963705062866, default) * 8f;
            blueDust.fadeIn = 0.5f;
            blueDust.noGravity = true;

            int moreBlue = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MoonBoulder, 0f, 0f, 0, default, 1f);
            Main.dust[moreBlue].velocity *= 0.5f;
            Main.dust[moreBlue].scale *= 1.3f;
            Main.dust[moreBlue].fadeIn = 1f;
            Main.dust[moreBlue].noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = (int)(128f * Projectile.scale);
            Projectile.height = (int)(128f * Projectile.scale);
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int i = 0; i < 64; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 10.5f);
            }
            for (int j = 0; j < 32; j++)
            {
                int killDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MoonBoulder, 0f, 0f, 100, default, 6.5f);
                Main.dust[killDust].noGravity = true;
                Main.dust[killDust].velocity *= 3f;
                killDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MoonBoulder, 0f, 0f, 100, default, 5.5f);
                Main.dust[killDust].velocity *= 2f;
                Main.dust[killDust].noGravity = true;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                for (int k = 0; k < 2; k++)
                {
                    int gored = Gore.NewGore(Projectile.GetSource_Death(), Projectile.position + new Vector2((float)(Projectile.width * Main.rand.Next(100)) / 100f, (float)(Projectile.height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, default, Main.rand.Next(61, 64), 1f);
                    Gore gore = Main.gore[gored];
                    gore.velocity *= 0.3f;
                    gore.velocity.X += (float)Main.rand.Next(-10, 11) * 0.05f;
                    gore.velocity.Y += (float)Main.rand.Next(-10, 11) * 0.05f;
                }
            }
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.localAI[1] = -1f;
                Projectile.maxPenetrate = 0;
                Projectile.Damage();
            }
            for (int l = 0; l < 50; l++)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    107,
                    259,
                    158
                });
                int exploding = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 2.5f * (float)Projectile.direction, -2.5f, 0, default, 5f);
                Main.dust[exploding].alpha = 200;
                Main.dust[exploding].velocity *= 2.4f;
                Main.dust[exploding].scale += Main.rand.NextFloat();
            }
        }
         
        // blurs the projectile
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}