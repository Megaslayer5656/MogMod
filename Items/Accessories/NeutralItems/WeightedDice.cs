using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class WeightedDice : NeutralItem
    {
        public const int RandDamage = 7;
        public const float RandGold = 0.3f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RandDamage, RandGold.ToPercent());
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 30;
            Item.height = 26;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingWeightedDice = true;
        }
    }
}