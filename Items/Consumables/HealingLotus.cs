using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Consumables
{
    public class HealingLotus : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useTurn = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/LotusConsume")
            {
                Volume = 1.1f,
                PitchVariance = 0.2f,
            };
            Item.consumable = true;
            Item.potion = true;
            Item.healMana = 80;
            Item.healLife = 40;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 0, 50, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.Moonglow, 3).
                AddIngredient(ItemID.FallenStar, 2).
                AddTile(TileID.Bottles).
                Register()
                .DisableDecraft();
        }
    }
}
