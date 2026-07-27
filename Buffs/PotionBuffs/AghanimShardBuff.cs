using MogMod.Items.Consumables;
using MogMod.Utilities;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Buffs.PotionBuffs
{
    public class AghanimShardBuff : ModBuff
    {
        public override LocalizedText Description => base.Description.WithFormatArgs(AghanimShard.ManaBoost, AghanimShard.MagicDamageBoost.ToPercent(), AghanimShard.ManaRegenBoost.ToRegenPerSecond());
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statManaMax2 += AghanimShard.ManaBoost;
            player.manaRegenBonus += AghanimShard.ManaRegenBoost;
            player.manaRegenDelay = 0f;
            player.GetDamage(DamageClass.Magic) += AghanimShard.MagicDamageBoost;
        }
    }
}