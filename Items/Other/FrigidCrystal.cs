using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class FrigidCrystal : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 22;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(silver: 5);
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient<FrigidShard>(3).
                AddIngredient(ItemID.CrystalShard, 1).
                AddIngredient(ItemID.SoulofLight, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}