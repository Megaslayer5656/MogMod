using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class EvilAssChaosStar : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 50;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
        }
        public override void AI()
        {
            Projectile.rotation += 0.25f;
            if (Main.rand.NextBool(3))
                for (int i = 0; i < 4; i++)
                {
                    int deathDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemRuby);
                    Main.dust[deathDust].noGravity = true;
                    Main.dust[deathDust].scale = 1.75f;
                }
        }
        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source) => SoundEngine.PlaySound(SoundID.Item117, Projectile.Center);
        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 70)
            {
                float timeAlpha = (float)Projectile.timeLeft / 10f;
                Projectile.alpha = (int)(255f - 255f * timeAlpha);
            }
            return new Color(255 - Projectile.alpha, 153 - Projectile.alpha, 204 - Projectile.alpha, 0);
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.position += Projectile.Size;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.position -= Projectile.Size;
            SoundEngine.PlaySound(SoundID.Item119, Projectile.Center);
            for (int i = 0; i < 5; i++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, 0f, 0f, 100, default, 1.2f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            if (!Main.dedServ)
                for (int i = 0; i < 3; i++)
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity * 0.05f, Main.rand.Next(16, 18), 1f);

            int choice = 10;
            float offset = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < choice; i++)
            {
                Vector2 velocity = ((MathHelper.TwoPi * i / choice) - offset).ToRotationVector2() * (choice / 2);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<EvilAssChaosStarProj>(), (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner);
            }
        }
    }
}