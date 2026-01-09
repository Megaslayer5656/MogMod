using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Buffs.Cooldowns
{
    public class DragonInstallCooldown : ModBuff
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
            //If I don't make a second debuff that applies on dragon install ending I could make negative effects here that are overshadowed by the dragon install buffs and have this have a long duration
        }
    }
}
