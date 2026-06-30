using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Buffs.Summons
{
    /// <summary>
    /// An abstract class that gives the baseline code needed to make a buff dedicated to a minion.
    /// </summary>
    public abstract class BaseSummonBuff : ModBuff
    {
        /// <summary>
        /// The correspondent minion's ID of this buff.
        /// </summary>
        protected abstract int MinionProjectileType { get; }

        /// <summary>
        /// The correspondent <see cref="MogPlayer"/> boolean field of this minion.
        /// </summary>
        protected abstract ref bool MinionBool { get; }

        /// <summary>
        /// The <see cref="Player"/> that possesses this buff (Read-only).
        /// </summary>
        protected Player BuffOwner { get; private set; }

        /// <summary>
        /// The <see cref="MogPlayer"/> that possesses this buff (Read-only).
        /// </summary>
        protected MogPlayer BuffModdedOwner { get; private set; }

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            BuffOwner = player;
            BuffModdedOwner = player.MogMod();

            if (player.ownedProjectileCounts[MinionProjectileType] > 0)
                MinionBool = true;

            if (!MinionBool)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }

            else
                player.buffTime[buffIndex] = 18000;
        }
    }
}