using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace MogMod.Items.Other
{
    public class UltimateOrb : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 15;
        public override void SetDefaults()
        {
            Item.height = Item.width = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 1, silver: 20);
        }

        public override void AddRecipes() //Might need to change the amount it makes
        {
            CreateRecipe(3).
                AddIngredient(ItemID.SoulofFright, 3).
                AddIngredient(ItemID.SoulofMight, 3).
                AddIngredient(ItemID.SoulofSight, 3).
                AddIngredient<ManaEssence>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
