using MogMod.Items.Other;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Consumables
{
    public class Cheese : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useTurn = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/CheeseConsume")
            {
                Volume = 1.1f,
                PitchVariance = 0.2f,
            };
            Item.consumable = true;
            Item.potion = true;
            Item.healMana = 300;
            Item.healLife = 170;
            Item.rare = ItemRarityID.Quest;
            Item.value = Item.buyPrice(0, 10, 30, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe(5).
                AddIngredient(ItemID.Stinkfish, 1).
                AddIngredient(ItemID.MilkCarton, 1).
                AddIngredient<UltimateOrb>(1).
                AddTile(TileID.Bottles).
                Register()
                .DisableDecraft();
        }
    }
}
