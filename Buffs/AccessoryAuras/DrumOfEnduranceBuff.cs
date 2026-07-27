using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories;
using MogMod.Utilities;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Buffs.AccessoryAuras
{
    public class DrumOfEnduranceBuff : ModBuff
    {
        public override LocalizedText Description => base.Description.WithFormatArgs(DrumOfEndurance.MovementSpeedBoost.ToPercent(), DrumOfEndurance.MeleeSpeedBoost.ToPercent());
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.drumsAura = true;
        }
    }
}