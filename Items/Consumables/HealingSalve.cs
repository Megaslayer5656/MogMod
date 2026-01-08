using MogMod.Common.Player;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using MogMod.Items.Other;

namespace MogMod.Items.Consumables
{
    public class HealingSalve : ModItem //, ILocalizedModType
    {
        //public new string LocalizationCategory => "Items.Misc";
        public static readonly SoundStyle salveUse = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/HealingSalveConsume")
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
            Item.rare = ItemRarityID.Green;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 0, 60, 0);
            Item.buffType = ModContent.BuffType<Buffs.PotionBuffs.HealingSalveBuff>();
            Item.buffTime = 1800;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(salveUse, player.Center);
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.HealingPotion, 3).
                AddIngredient(ItemID.Moonglow, 2).
                AddIngredient(ItemID.Daybloom, 1).
                AddTile(TileID.Bottles).
                Register();
        }
    }
}