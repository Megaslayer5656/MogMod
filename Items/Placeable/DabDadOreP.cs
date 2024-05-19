using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable
{
    public class DabDadOreP : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.DabDadOre>());
            Item.width = 12;
            Item.height = 12;
            Item.value = 3000;
            Item.rare = ItemRarityID.Green;
        }
    }
}