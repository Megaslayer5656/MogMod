using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class ScorchedCore : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 1);
        }
    }
}