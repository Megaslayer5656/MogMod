using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.ClasslessProjectiles
{
    public class PolyluteProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.ClasslessProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        private float wSpeed = 0f;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ArmorPenetration = 30;
        }

        public override void AI()
        {
            Projectile.ai[2]++;

            Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.ShimmerSpark, -Projectile.velocity * Main.rand.NextFloat(0.05f, 0.6f));
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.5f, 0.8f);
            dust.color = Main.rand.NextBool(3) ? Color.DarkViolet : Color.Purple;

            if (wSpeed == 0f)
                wSpeed = Projectile.velocity.Length();

            if (Projectile.ai[2] >= 10)
            {
                MogModUtils.HomeInOnNPC(Projectile, true, 200f, wSpeed, 1f);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[2] = 1f;
        }
        public override void OnKill(int timeLeft)
        {
            //TODO: Make it explode on kill
        }
    }
}