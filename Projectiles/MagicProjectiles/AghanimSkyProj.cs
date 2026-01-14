using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class AghanimSkyProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        private bool initialized = false;
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 200;
            Projectile.MaxUpdates = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

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
                    int i = Dust.NewDust(Projectile.Center, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, Color.BlueViolet, 1f);
                    Main.dust[i].scale = 1.5f;
                    Main.dust[i].noGravity = true;
                    Main.dust[i].position = Projectile.Center + offset;
                    Main.dust[i].velocity = Projectile.velocity * 0f + offset.SafeNormalize(Vector2.UnitY) * 1f;
                    d++;
                }
            }

            MogModUtils.HomeInOnNPC(Projectile, true, 800f, 20f, 15f);

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
                    Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.RainbowMk2, Projectile.velocity, 100, Color.Violet, 1f);
                    dust.noGravity = true;
                    dust.scale = 0.75f;
                    dust.position = Projectile.Center + offset;
                    dust.velocity = Projectile.velocity;
                }
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 0f)
            {
                for (int d = 0; d < 4; d++)
                {
                    Vector2 source = Projectile.position;
                    source -= Projectile.velocity * ((float)d * 0.25f);
                    Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.RainbowTorch, Projectile.velocity, 100, Color.BlueViolet, 1f);
                    dust.noGravity = true;
                    dust.position = source;
                    dust.scale = Main.rand.NextFloat(0.91f, 1.417f);
                    dust.velocity *= 0.1f;
                }
            }
            if (Projectile.timeLeft < 50)
            {
                Projectile.tileCollide = true;
            }
        }
        public override void OnKill(int timeLeft)
        {
            var source = Projectile.GetSource_FromThis();
            Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AghanimExplosion>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 16;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            int dustAmt = 24;
            for (int j = 0; j < dustAmt; j++)
            {
                Vector2 dustRotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f; //0.75
                dustRotate = dustRotate.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustDirection = dustRotate - Projectile.Center;
                int killDust = Dust.NewDust(dustRotate + dustDirection, 0, 0, DustID.RainbowTorch, dustDirection.X, dustDirection.Y, 100, Color.BlueViolet, 1f);
                Main.dust[killDust].noGravity = true;
                Main.dust[killDust].noLight = true;
                Main.dust[killDust].velocity = dustDirection;
            }
            for (int j = 0; j < dustAmt; j++)
            {
                Vector2 dustRotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.5f;
                dustRotate = dustRotate.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustDirection = dustRotate - Projectile.Center;
                int killDust = Dust.NewDust(dustRotate + dustDirection, 0, 0, DustID.RainbowTorch, dustDirection.X, dustDirection.Y, 100, Color.MediumPurple, 1f);
                Main.dust[killDust].noGravity = true;
                Main.dust[killDust].noLight = true;
                Main.dust[killDust].velocity = dustDirection;
            }
        }
    }
}