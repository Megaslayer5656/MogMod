using System;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using MogMod.Utilities;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace MogMod.Projectiles.Melee
{
    public class SelvesProj2 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public bool canHit = true;
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.scale = 1f;
            Projectile.netUpdate = true;
            Projectile.alpha = 150;
            Projectile.aiStyle = ProjAIStyleID.Arrow;

            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            Projectile.netUpdate = true;
            Projectile.rotation += MathHelper.ToRadians(-45f);
            Projectile.alpha = 150;
            Projectile.ai[2] = 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft = 20;
            MogModUtils.ProjectileBarrage(Projectile.GetSource_FromThis(), target.Center, target.Center, Main.rand.NextBool(), 150f, 150f, -150f, 150f, 10f, ModContent.ProjectileType<SelvesProj3>(), Convert.ToInt32(Projectile.damage * 0.95), 0f, Projectile.owner, false, 0f);
            canHit = false;
        }

        public override bool? CanHitNPC(NPC target) => canHit ? null : false;

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 1, 1, DustID.ShimmerSpark, 0, 0, 0, default, 1f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 1, 1, DustID.ShimmerSpark, 0, 0, 0, default, 1f);
            }
        }
    }

}