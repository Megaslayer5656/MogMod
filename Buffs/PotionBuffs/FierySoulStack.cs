using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Buffs.PotionBuffs
{
    public class FierySoulStack : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
        }
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            Player player = Main.LocalPlayer;
            MogPlayer mogPlayer = player.MogMod();
            tip = base.Description.Format(mogPlayer.fierySoulLevel);
        }
    }
}