using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable.Ores
{
    public class DabDadOreP : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.SortingPriorityMaterials[Type] = 90; // Chlorophyte Ore
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Ores.DabDadOre>());
            Item.width = 12;
            Item.height = 12;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(silver: 24);
        }
    }
}