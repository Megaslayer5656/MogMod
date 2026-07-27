using MogMod.Utilities;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Buffs.PotionBuffs
{
    public class ParryBuff1 : ModBuff
    {
        public const float ParryDamage = 0.25f;
        public const float ParrySpeed = 0.15f;
        public override LocalizedText Description => base.Description.WithFormatArgs(ParrySpeed.ToPercent(), ParryDamage.ToPercent());
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetAttackSpeed<MeleeDamageClass>() += .25f;
            player.GetDamage<MeleeDamageClass>() += .15f;
        }
    }
}
