using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories;
using MogMod.Utilities;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Buffs.AccessoryAuras
{
    public class HeaddressBuff : ModBuff
    {
        public override LocalizedText Description => base.Description.WithFormatArgs(Headdress.LifeRegenBoost.ToRegenPerSecond());
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.headdressAura = true;
        }
    }
}