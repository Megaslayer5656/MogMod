using Terraria;
using Terraria.ModLoader;
namespace MogMod.Buffs.PotionBuffs
{
    public class ETGCbuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 6;
        }
    }
}