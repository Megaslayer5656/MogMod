using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class SearingSignet : NeutralItem
    {
        public const float NonMeleeDamageBoost = 0.07f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(NonMeleeDamageBoost.ToPercent());
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingSearingSignet = true;

            player.GetDamage(DamageClass.Ranged) += NonMeleeDamageBoost;
            player.GetDamage(DamageClass.Magic) += NonMeleeDamageBoost;
            player.GetDamage(DamageClass.Summon) += NonMeleeDamageBoost;
        }
    }
}