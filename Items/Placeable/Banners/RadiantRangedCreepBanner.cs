using MogMod.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable.Banners
{
    public class RadiantRangedCreepBanner : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EnemyBanner>(), (int)EnemyBanner.StyleID.RadiantRangedCreep);
            Item.width = 10;
            Item.height = 24;
            Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(silver: 10));
        }
    }
}