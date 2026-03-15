using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.ClasslessProjectiles
{
    public class PlasmaShrimpProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.ClasslessProjectiles";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ArmorPenetration = 20;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 590)
                MogModUtils.HomeInOnNPC(Projectile, true, 1500f, 10f, 25f);

            Projectile.rotation += 0.3f * (float)Projectile.direction;

            // dust id's 71 - 73 are nebula slop
            if (Main.rand.NextBool(5))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 71, 0, 0, 0, default, .75f);
            }

            if (Main.rand.NextBool(4))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 72, 0, 0, 0, default, .75f);
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 73);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft < 590)
                return true;
            else
                return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath39, Projectile.Center);
        }
    }
}