using MogMod.Projectiles.SummonerProjectiles;
using Terraria.ModLoader;

namespace MogMod.Buffs.Summons
{
    public class FrostCrystalSummonBuff : BaseSummonBuff
    {
        protected override int MinionProjectileType => ModContent.ProjectileType<FrostCrystalSummon>();
        protected override ref bool MinionBool => ref BuffModdedOwner.fCrystal;
    }
}