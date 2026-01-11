using MogMod.Common.MogModPlayer;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Buffs.PotionBuffs
{
    public class MagicStickBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            player.statLife += 5 * mogPlayer.stickCharges;
            player.statMana += 5 * mogPlayer.stickCharges;
            mogPlayer.stickCharges = 0;
        }
    }
}