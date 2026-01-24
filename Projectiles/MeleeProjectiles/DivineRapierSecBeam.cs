using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class DivineRapierSecBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 70;
            Projectile.timeLeft = 100;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 70)
                Projectile.ai[1] = 1f;
            if (Projectile.ai[1] >= 1f)
            {
                MogModUtils.HomeInOnNPC(Projectile, true, 800f, 20f, 15f);
                Projectile.extraUpdates = 70;
            }
            float pi = MathHelper.Pi;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] == 48f)
            {
                Projectile.ai[0] = 0f;
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 0f)
            {
                for (int d = 0; d < 2; d++)
                {
                    Vector2 offset = Vector2.UnitX * -1f;
                    offset = -Vector2.UnitY.RotatedBy((double)(Projectile.ai[0] * pi / 12f + (float)d * pi), default) * new Vector2(5f, 10f) - Projectile.rotation.ToRotationVector2() * 10f;
                    Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.ShimmerSpark, Projectile.velocity, 100, Color.White, 1f);
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.91f, 1.417f);
                    dust.position = Projectile.Center + offset;
                    dust.velocity = Projectile.velocity;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<DivineMightDebuff>(), 600);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<DivineMightDebuff>(), 600);
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft >= 70)
            {
                return false;
            }
            return null;
        }
        public override bool CanHitPvp(Player target) => Projectile.timeLeft < 70;
    }
}