using Microsoft.Xna.Framework;
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
        public new string LocalizationCategory => "Projectiles.SummonerProjectiles";
        public static readonly SoundStyle mineActivate = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/ProximityMineActivate")
        {
            Volume = 1.3f,
            PitchVariance = .2f,
            MaxInstances = 0
        };
        public ref float ExplosionTimer => ref Projectile.ai[1];
        private bool initialized = false;
        private bool Exploding = false;
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
            float detectionRadius = 100f;

            // checks for enemies nearby
            for (int i = 0; i < Main.maxNPCs; i++) //Every npc is in an index, this goes through all of them
            {
                NPC otherNPC = Main.npc[i]; //This sets the var otherNPC to the current npc we are targeting in the index
                if (otherNPC.active && !otherNPC.friendly && otherNPC.whoAmI != otherNPC.whoAmI - 1) //Makes shivas not hit inactive npcs, townNpcs, and not cast on the same npc twice.
                {
                    if (Vector2.Distance(Projectile.Center, Main.npc[i].Center) < detectionRadius)
                    {
                        // start exploding
                        Exploding = true;
                    }
                }
            }
            if (Exploding)
            {
                if (!initialized)
                {
                    SoundEngine.PlaySound(mineActivate, Projectile.Center);
                    initialized = true;
                }
                // counts down explosion timer
                ExplosionTimer += 1f;
                Dust.NewDust(Projectile.Center + Projectile.velocity, Projectile.width, Projectile.height, DustID.Flare, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
                // after a certain time explode
                if (Main.myPlayer == Projectile.owner && ExplosionTimer >= 60)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<ProximityMinesExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
                    Projectile.Kill();
                }
            }
            // gravity
            Projectile.velocity.Y += 0.5f;
            if (Projectile.velocity.Y > 10f)
            {
                Projectile.velocity.Y = 10f;
            }
        }
        public override bool? CanDamage() => false;
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public override void OnKill(int timeLeft)
        {
            int dustsplash = 0;
            while (dustsplash < 4)
            {
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 0.9f);
                Main.dust[d].position = Projectile.Center;
                dustsplash += 1;
            }
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
    }
}