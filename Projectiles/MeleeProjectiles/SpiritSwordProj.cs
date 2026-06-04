using MogMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class SpiritSwordProj : ModProjectile
    {
        bool canHitNPC = false;
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.damage = 40;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
        }
        public override void AI()
        {
            Projectile.ai[1]++;

            if (Projectile.ai[1] < 30)
            {
                Projectile.velocity *= .9f;
            }

            if (Projectile.ai[1] > 30)
            {
                canHitNPC = true;
                MogModUtils.HomeInOnNPC(Projectile, false, 2400, 10f, 10f);
            }


            Dust dust2 = Dust.NewDustPerfect(Projectile.position, DustID.SilverCoin, Projectile.velocity, 100, default, 1.87f);
            dust2.noGravity = true;
            dust2.scale = Main.rand.NextFloat(1.617f, 2.1f);
            dust2.velocity *= 0.1f;

            Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.SilverCoin, Projectile.velocity, 100, default, 1.2f);
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.9f, 1.217f);
            dust.velocity *= 0.1f;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin, 0f, 0f, 100, default, 1.2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity -= Projectile.oldVelocity * 0.3f;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return canHitNPC;
        }
    }
}