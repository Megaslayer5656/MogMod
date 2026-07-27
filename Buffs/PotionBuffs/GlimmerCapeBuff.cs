using MogMod.Items.Accessories;
using MogMod.Utilities;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Buffs.PotionBuffs
{
    public class GlimmerCapeBuff : ModBuff
    {
        public override LocalizedText Description => base.Description.WithFormatArgs(GlimmerCape.MovementSpeedBoost.ToPercent(), GlimmerCape.ManaRegenBoost.ToRegenPerSecond(), GlimmerCape.BuffAggroBoost);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.invis = true;
            player.moveSpeed += GlimmerCape.MovementSpeedBoost;
            player.manaRegenBonus += GlimmerCape.ManaRegenBoost;
            player.manaRegenDelay -= 4f;
            player.aggro -= GlimmerCape.BuffAggroBoost;
        }
    }
}