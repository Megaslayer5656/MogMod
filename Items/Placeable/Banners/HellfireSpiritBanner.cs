using MogMod.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable.Banners
{
    public class HellfireSpiritBanner : ModItem
	{
		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<EnemyBanner>(), (int)EnemyBanner.StyleID.HellfireSpirit);
			Item.width = 10;
			Item.height = 24;
			Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(silver: 10));
		}
	}
}