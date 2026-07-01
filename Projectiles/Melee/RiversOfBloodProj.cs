using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class RiversOfBloodProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults() //TODO: Fix this projectile's janky hitbox
        {
            Projectile.width = 76;
            Projectile.height = 30;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.scale = 2f;
            Projectile.netUpdate = true;

            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.position, 1f, 0f, 0f);
            if (Main.rand.NextBool(5))
            {
                int blood = Dust.NewDust(Projectile.position, 10, 60, DustID.Blood, 0f, 0f, 150, default, 3f);
            }

            Projectile.netUpdate = true;
        }
    }
}
