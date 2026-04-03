using MogMod.Common.MogModPlayer;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Buffs.PotionBuffs
{
    public class HolyLocketBuff : ModBuff
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
            int heal = 10 * mogPlayer.locketCharges;

            player.statLife += heal;
            player.HealEffect(heal);
            if (player.statLife > player.statLifeMax2)
            {
                player.statLife = player.statLifeMax2;
            }

            player.statMana += heal;
            player.ManaEffect(heal);
            if (player.statMana > player.statManaMax2)
            {
                player.statMana = player.statManaMax2;
            }

            mogPlayer.locketCharges = 0;
        }
    }
}
