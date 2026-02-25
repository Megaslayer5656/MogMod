using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Items.Other
{
    public class LedX : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetDefaults()
        {
            Item.width = 5;
            Item.height = 5;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Red;
            Item.value = 10000000;
        }
    }
}