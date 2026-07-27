using MogMod.Items.Consumables;
using MogMod.Utilities;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
namespace MogMod.Buffs.PotionBuffs
{
    public class HealingSalveBuff : ModBuff
    {
        public override LocalizedText Description => base.Description.WithFormatArgs(HealingSalve.LifeRegenBoost.ToRegenPerSecond());
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += HealingSalve.LifeRegenBoost;
        }
    }
}