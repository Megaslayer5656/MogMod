using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class HellfireEssence : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 22;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 1, copper: 50);
        }
    }
}