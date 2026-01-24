using MogMod.Common.MogModPlayer;
using MogMod.NPCs.Global;
using MogMod.Projectiles.BaseProjectiles;
using Terraria;

namespace MogMod.Utilities
{
    public static partial class MogModUtils
    {
        public static MogPlayer MogMod(this Player player) => player.GetModPlayer<MogPlayer>();
        public static MogModGlobalNPC MogMod(this NPC npc) => npc.GetGlobalNPC<MogModGlobalNPC>();
        public static MogModGlobalProjectile MogMod(this Projectile proj) => proj.GetGlobalProjectile<MogModGlobalProjectile>();
    }
}
