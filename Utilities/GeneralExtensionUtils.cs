using MogMod.Projectiles;
using Terraria;

namespace MogMod.Utilities
{
    public static partial class MogModUtils
    {
        public static MogModGlobalProjectile MogMod(this Projectile proj) => proj.GetGlobalProjectile<MogModGlobalProjectile>();
    }
}
