using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class GunlanceBoom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";

        private const float radius = 50f;

        public override void SetDefaults()
        {
            Projectile.width = (int)radius*2;
            Projectile.height = (int)radius*2;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            for (int i = 0; i < 10; i++)
            {
                Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                dust2.scale = Main.rand.NextFloat(2.2f, 2.5f);
                dust2.noGravity = true;
            }
            for (int i = 0; i < 5; i++)
            {
                Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
                dust2.scale = Main.rand.NextFloat(1.7f, 2f);
                dust2.noGravity = true;
            }
        }
        public override void OnKill(int timeLeft) => Projectile.Damage();
        public override void OnSpawn(IEntitySource source) => SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => MogModUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);
    }
}