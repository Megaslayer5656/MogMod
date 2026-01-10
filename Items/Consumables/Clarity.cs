using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Consumables
{
    public class Clarity : ModItem
    {
        public static readonly SoundStyle clarityUse = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/ClarityConsume")
        {
            Volume = 1.1f,
            PitchVariance = .2f,
            MaxInstances = 3
        };
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 30;
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useAnimation = Item.useTime = 15;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 0, 50, 0);
            Item.buffType = ModContent.BuffType<Buffs.PotionBuffs.ClarityBuff>();
            Item.buffTime = 1800;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(clarityUse, player.Center);
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.ManaPotion, 3).
                AddIngredient(ItemID.Shiverthorn, 2).
                AddIngredient(ItemID.Waterleaf, 1).
                AddTile(TileID.Bottles).
                Register();
        }
    }
}