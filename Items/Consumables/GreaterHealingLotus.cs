using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Consumables
{
    public class GreaterHealingLotus : ModItem
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
            Item.healMana = 220;
            Item.healLife = 140;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(0, 4, 60, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient<GreatHealingLotus>(1).
                AddIngredient(ItemID.PixieDust, 1).
                AddIngredient(ItemID.HallowedSeeds, 2).
                AddTile(TileID.Bottles).
                Register()
                .DisableDecraft();
            CreateRecipe(3).
                AddIngredient(ItemID.UnicornHorn, 1).
                AddIngredient(ItemID.SoulofLight, 1).
                AddTile(TileID.Bottles).
                Register()
                .DisableDecraft();
        }
    }
}
