using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class CreepBlood : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 22;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(copper: 20);
        }
    }
}