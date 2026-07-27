using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories.Boots;
using MogMod.Utilities;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Buffs.AccessoryAuras
{
    public class GuardianGreavesAura : ModBuff
    {
        public override LocalizedText Description => base.Description.WithFormatArgs(GuardianGreaves.LifeRegenBoost.ToRegenPerSecond(), GuardianGreaves.DefenseBoost, GuardianGreaves.LifeBoost, GuardianGreaves.ManaBoost, GuardianGreaves.MagicDamageBoost.ToPercent());
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.greavesAura = true;
            mogPlayer.headdressAura = true;
        }
    }
}