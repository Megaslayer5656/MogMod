using MogMod.Utilities;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using MogMod.NPCs.Global;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class MarkerProjProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.damage = 50;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            Projectile.ai[2]++;

            float maxSpeed = 6;
            float currentSpeed = Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y;
            if (currentSpeed < maxSpeed * maxSpeed)
            {
                Projectile.velocity *= 2.2f;
            }
            if (Projectile.ai[2] > 30)
            {
                if (MogModUtils.AnyMarkedNPCAlive())
                {
                    // Only home to marked NPC
                    MogModUtils.HomeInOnMarkedNPC(Projectile, true, 3000f, 11f, 10f);
                }
                else
                {
                    // No marked NPC exists — fallback to normal homing
                    MogModUtils.HomeInOnNPC(Projectile, true, 800f, 11f, 10f);
                }
            }


            Dust dust2 = Dust.NewDustPerfect(Projectile.position, DustID.IchorTorch, Projectile.velocity, 100, default, 1.87f);
            dust2.noGravity = true;
            dust2.scale = Main.rand.NextFloat(1.617f, 2.1f);
            dust2.velocity *= 0.1f;

            Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.IchorTorch, Projectile.velocity, 100, default, 1.2f);
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.9f, 1.217f);
            dust.velocity *= 0.1f;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IchorTorch, 0f, 0f, 100, default, 1.2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity -= Projectile.oldVelocity * 0.3f;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            bool markedAlive = MogModUtils.AnyMarkedNPCAlive();

            if (!markedAlive)
                return null; // allow normal behavior

            if (!target.TryGetGlobalNPC<MogModGlobalNPC>(out var globalNPC))
                return false;

            return globalNPC.markedByMarker;
        }
    }
}
