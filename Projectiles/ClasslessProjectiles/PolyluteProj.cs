using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
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
            Projectile.width = Projectile.height = 4;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ArmorPenetration = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.ai[2]++;

            Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.AncientLight, Projectile.velocity, 100, Color.DeepPink, 1.5f);
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(1.1f, 1.617f);
            dust.velocity *= 0.1f;

            if (wSpeed == 0f)
                wSpeed = Projectile.velocity.Length();

            if (Projectile.ai[2] >= 10)
            {
                MogModUtils.HomeInOnNPC(Projectile, true, 200f, wSpeed, 1f);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //TODO: give dust effect
            Projectile.ai[2] = 1f;
        }
        public override void OnKill(int timeLeft)
        {
            //TODO: give dust effect
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.NPCHit36, Projectile.Center);
        }
    }
}