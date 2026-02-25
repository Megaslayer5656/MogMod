using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using MogMod.UI;
using MogMod.Common.Interfaces;

namespace MogMod.Items.Accessories
{
    public class IdeaRig : ModItem, ILocalizedModType, IRigItem
    {
        public new string LocalizationCategory => "Items.Accessories";
        public CustomItemSlot idearigslot;
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = 25000;
            Item.waistSlot = 1;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.ammoCost75 = true;
            player.lifeRegen = 1;
        }
    }
}
