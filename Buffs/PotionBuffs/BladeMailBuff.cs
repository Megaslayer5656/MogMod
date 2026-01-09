using Terraria;
using Terraria.ModLoader;
namespace MogMod.Buffs.PotionBuffs
{
    public class BladeMailBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.thorns += 10f;
        }
    }
}