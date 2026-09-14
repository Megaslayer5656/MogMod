using MogMod.Items.Global;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class BladesOfAttack : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int CritBoost = 3;
        public const int FlatDamageBoost = 2;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBoost, FlatDamageBoost);
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.White;
            Item.value = MogGlobalItem.RarityWhiteBuyPrice;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance<GenericDamageClass>() += CritBoost;
            player.GetDamage<GenericDamageClass>().Flat += FlatDamageBoost;
        }
    }
}
