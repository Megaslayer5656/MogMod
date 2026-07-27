using MogMod.Items.Consumables;
using MogMod.Utilities;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
namespace MogMod.Buffs.PotionBuffs
{
    public class MoonShardBuff : ModBuff
    {
        public override LocalizedText Description => base.Description.WithFormatArgs(MoonShard.AttackSpeedBoost.ToPercent());
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetAttackSpeed(DamageClass.Generic) += MoonShard.AttackSpeedBoost;
        }
    }
}