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
<<<<<<<< HEAD:Buffs/BladeMailBuff.cs
            player.thorns += 3f;
========
            player.thorns += 10f;
>>>>>>>> e4ed66c01e9b6963a3de4abdf8f41f9c8ab41a35:Buffs/PotionBuffs/BladeMailBuff.cs
        }
    }
}