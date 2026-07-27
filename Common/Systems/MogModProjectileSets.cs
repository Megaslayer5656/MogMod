using MogMod.Projectiles.EnemyProjectiles.Boss;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Common.Systems
{
    [ReinitializeDuringResizeArrays]
    public static class MogModProjectileSets
    {
        private static SetFactory Factory = ProjectileID.Sets.Factory;
        /// <summary>
        /// If <see langword="true"/> for a projectile type, then that projectile will never be reflected by armor or accessory effects.<br/>
        /// Set this for persistent projectiles such as deathrays to avoid major screwing of their behavior.<br/>
        /// Only needs to be set for hostile projectiles, as these effects already have a check to ensure they never trigger in PvP.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] ShouldNotBeReflected = Factory.CreateNamedSet("ShouldNotBeReflected")
            .Description("Prevents this projectile from being reflected by armor or accessory effects.")
            .RegisterBoolSet(ProjectileID.SaucerDeathray, ProjectileID.PhantasmalDeathray, ProjectileType<VonLaserEyes>(), ProjectileType<VonLaserSpawner>(),
                ProjectileType<VonTargetLaser>());
    }
}
