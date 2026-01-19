using Microsoft.Xna.Framework;
using MogMod.Projectiles.MeleeProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.SummonerProjectiles
{
    // ExampleSentry is an example sentry.
    // ExampleSentry demonstrates both floating and grounded sentry behaviors. Use ExampleSentryItem to the left of the player spawn a grounded sentry and use it to the right to spawn a floating sentry.
    // Sentries are similar to Minions, but typically don't move, are limited by the sentry limit instead of the minion limit, don't have a corresponding buff, and last for 10 minutes instead of despawning when the player dies.
    // The most critical fields necessary for a projectile to count as a sentry will be noted in this file and in ExampleSentryItem.cs. See also ExampleSentryShot.cs.
    public class ProximityMinesSummon : ModProjectile, ILocalizedModType
    {
        // currently doeesnt explode when a goon is nearby, does it on spawn instead
        public new string LocalizationCategory => "Projectiles.SummonerProjectiles";
        public const float MinExplodeDistance = 120f;
        public const float ExplodeWaitTime = 45f;
        public const float ExplosionAngleVariance = 0.8f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 28;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override void OnSpawn(IEntitySource source)
        { 
            SoundEngine.PlaySound(SoundID.DD2_DefenseTowerSpawn, Projectile.Center);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            bool canExplode = Projectile.Center.ClosestNPCAt(MinExplodeDistance) != null;
            if (player.HasMinionAttackTargetNPC)
            {
                canExplode = Main.npc[player.MinionAttackTargetNPC].Distance(Projectile.Center) < MinExplodeDistance;
            }
            if (canExplode)
                Projectile.ai[0]++;
            if (Projectile.ai[0] >= ExplodeWaitTime)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0f, -Main.rand.NextFloat(6f, 10f)).RotatedByRandom(ExplosionAngleVariance), ModContent.ProjectileType<DaedalusBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                for (int k = 0; k < 5; k++)
                {
                    Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Flare, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
                }
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
                Projectile.Kill();
            }
            Projectile.velocity.Y += 0.5f;
            if (Projectile.velocity.Y > 10f)
            {
                Projectile.velocity.Y = 10f;
            }
        }
        public override bool? CanDamage() => false;
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
    }
}