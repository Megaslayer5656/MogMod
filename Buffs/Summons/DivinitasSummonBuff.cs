using MogMod.Projectiles.Summon;
using Terraria.ModLoader;

namespace MogMod.Buffs.Summons
{
    public class DivinitasSummonBuff : BaseSummonBuff
    {
        protected override int MinionProjectileType => ModContent.ProjectileType<DivinitasSummon>();
        protected override ref bool MinionBool => ref BuffModdedOwner.divinitasMinion;
    }
}