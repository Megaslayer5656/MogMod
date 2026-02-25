using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class GriefBar : ModItem, ILocalizedModType
    {
        // TODO: make this placeable like other bars
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 15;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Pink;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellstoneBar, 1).
                AddIngredient(ItemID.SoulofFright, 1).
                AddIngredient(ItemID.SoulofNight, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}