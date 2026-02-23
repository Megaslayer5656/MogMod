using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class LAS13Proj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 6f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustSpawnPos = Projectile.position - Projectile.velocity * i / 2f;
                    Dust lasDust = Dust.NewDustPerfect(dustSpawnPos, DustID.AncientLight);
                    lasDust.color = Color.Lerp(Color.Yellow, Color.Gold, Main.rand.NextFloat(0.6f));
                    lasDust.scale = Main.rand.NextFloat(0.96f, 1.04f);
                    lasDust.noGravity = true;
                    lasDust.velocity *= 0.1f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            int dustAmt = 6;
            for (int j = 0; j < dustAmt; j++)
            {
                Vector2 dustRotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                dustRotate = dustRotate.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustDirection = dustRotate - Projectile.Center;
                int killDust = Dust.NewDust(dustRotate + dustDirection, 0, 0, DustID.AncientLight, dustDirection.X, dustDirection.Y, 100, Color.Yellow, 1.2f);
                Main.dust[killDust].noGravity = true;
                Main.dust[killDust].noLight = true;
                Main.dust[killDust].velocity = dustDirection;
            }
        }
    }
}