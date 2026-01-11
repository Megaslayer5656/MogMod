using MogMod.Common.MogModPlayer;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Buffs.PotionBuffs
{
    public class WandBuff : ModBuff
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
            player.statLife += 7 * mogPlayer.wandCharges;
            player.statMana += 7 * mogPlayer.wandCharges;
            mogPlayer.wandCharges = 0;
        }
    }
}
