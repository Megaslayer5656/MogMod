using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Other
{
    public class SoulOfMogMod : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            Item.ResearchUnlockCount = 5;
        }
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 5);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                DisableDecraft().
                AddIngredient<ManaCore>().
                AddIngredient<ScorchedCore>().
                AddIngredient(ItemID.FrostCore).
                AddIngredient<FrostEssence>().
                AddIngredient<SpookyEssence>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}