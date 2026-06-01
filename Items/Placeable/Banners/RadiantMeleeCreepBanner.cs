using MogMod.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace MogMod.Items.Placeable.Banners
{
    public class RadiantMeleeCreepBanner : ModItem
	{
		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<EnemyBanner>(), (int)EnemyBanner.StyleID.RadiantMeleeCreep);
			Item.width = 10;
			Item.height = 24;
			Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(silver: 10));
		}
	}
}