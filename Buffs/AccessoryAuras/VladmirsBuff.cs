using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories;
using MogMod.Utilities;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Buffs.AccessoryAuras
{
    public class VladmirsBuff : ModBuff
    {
        public override LocalizedText Description => base.Description.WithFormatArgs(VladmirsOffering.DefenseBoost, VladmirsOffering.FlatDamageBoost, VladmirsOffering.ManaRegenBoost.ToRegenPerSecond(), VladmirsOffering.LifeStealBoost.ToPercent());
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.vladsAura = true;
        }
    }
}