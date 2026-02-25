using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace MogMod.Items.Other
{
    public class LizhardBloodVial : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 31;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Red;
            Item.value = 1000;
        }
    }
}
