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
            int heal = 5 * mogPlayer.stickCharges;

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

                mogPlayer.stickCharges = 0;
                player.DelBuff(buffIndex);
        }
    }
}