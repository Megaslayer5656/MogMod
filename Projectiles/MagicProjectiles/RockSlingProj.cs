using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class RockSlingProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 36;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            var mogPlayer = Main.LocalPlayer.GetModPlayer<MogPlayer>();
            float maxSpeed = 25;
            float currentSpeed = Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y;

            if (mogPlayer.holdingMeteoriteStaff)
                MogModUtils.HomeInOnNPC(Projectile, false, 400, currentSpeed / 40f, 10f);
            Projectile.rotation += currentSpeed * 0.15f;
            Projectile.localAI[1] += 1f;
            if (Projectile.timeLeft < 570)
                Projectile.ai[0] = 1f;

            if (Projectile.ai[0] >= 1f)
            {
                if (currentSpeed < maxSpeed * maxSpeed)
                {
                    Projectile.velocity *= 1.17f;
                }
                Projectile.ai[0] = 0f;
                Projectile.tileCollide = true;
            }
            else
            {
                Projectile.velocity *= 0.99f;
                Projectile.tileCollide = false;
            }

            int rockDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, 0f, 0f, 0, default, 1f);
            Main.dust[rockDust].velocity *= 0.5f;
            Main.dust[rockDust].scale *= 1.05f;
            Main.dust[rockDust].fadeIn = 0.7f;
            Main.dust[rockDust].noGravity = true;
        }
        public override void OnKill(int timeLeft)
        {
            int numb = 2;
            Projectile.position = Projectile.Center;
            Projectile.width *= 2;
            Projectile.height *= 2;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width / numb, Projectile.height / numb, DustID.PurpleCrystalShard, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 1.2f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int i = 0; i < 10; i++)
            {
                int dusty = Dust.NewDust(Projectile.position, Projectile.width / numb, Projectile.height / numb, DustID.PurpleCrystalShard, 0f, 0f, 100, default, 3f);
                Main.dust[dusty].noGravity = true;
                Main.dust[dusty].velocity *= 1.3f;
                dusty = Dust.NewDust(Projectile.position, Projectile.width / numb, Projectile.height / numb, DustID.PurpleCrystalShard, 0f, 0f, 100, default, 2f);
                Main.dust[dusty].velocity *= 1.1f;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.localAI[1] = -1f;
                Projectile.maxPenetrate = 0;
                Projectile.Damage();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}