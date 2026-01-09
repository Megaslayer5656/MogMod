using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Buffs.Cooldowns
{
    public class LagunaBladeCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
        }
    }
}