using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class ScavVest : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const float MovementSpeedBoost = 0.08f;
        public const float ConsumeAmmoChance = 0.94f;
        public const int FishingPowerBoost = 5;
        public const int LureCount = 2;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MovementSpeedBoost.ToPercent(), ConsumeAmmoChance.ToReversedPercent(), FishingPowerBoost, LureCount);
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;

            Item.accessory = true;

            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingScavVest = true;
            mogPlayer.ammoCost *= ConsumeAmmoChance;
            player.moveSpeed += MovementSpeedBoost;
            player.fishingSkill += FishingPowerBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => tooltips.FindAndReplace("Scav Vest", this.GetLocalizedValue(Main.zenithWorld ? "NameGFB" : "NameNormal"));
    }
}