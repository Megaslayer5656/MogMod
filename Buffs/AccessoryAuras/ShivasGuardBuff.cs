using Terraria;
using Terraria.ModLoader;
namespace MogMod.Buffs.AccessoryAuras
{
    public class ShivasGuardBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 10;
            player.GetDamage(DamageClass.Ranged) += .20f;
        }
    }
}