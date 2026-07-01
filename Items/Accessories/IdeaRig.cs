using MogMod.Common.Interfaces;
using MogMod.Common.MogModPlayer;
using MogMod.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.ammoCost *= 0.75f;
            player.lifeRegen = 1;
        }
    }
}
