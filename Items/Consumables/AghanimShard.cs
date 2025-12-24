using MogMod.Common.Player;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using MogMod.Items.Other;

namespace MogMod.Items.Consumables
{
    public class AghanimShard : ModItem //, ILocalizedModType
    {
        //public new string LocalizationCategory => "Items.Misc";
        public static readonly SoundStyle aghanimUse = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/AghanimShard")
        {
            Volume = 1.1f,
            PitchVariance = .2f
        };
        public static readonly int ManaBoost = 100;
        public static readonly int ManaBoostMax = 100;
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useAnimation = 30;
            Item.rare = ItemRarityID.Cyan;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ConsumedManaCrystals == Player.ManaCrystalMax;
        }

        public override bool? UseItem(Player player)
        {
            // for some reason it doesnt stop the player from using it multiple times
            SoundEngine.PlaySound(aghanimUse, player.Center);
            if (player.GetModPlayer<MogPlayerStatIncrease>().aghanimShard >= ManaBoostMax)
            {
                // Returning null will make the item not be consumed
                return null;
            }
            // This method handles permanently increasing the player's max mana and displaying the blue mana text
            player.UseManaMaxIncreasingItem(ManaBoost);
            // This field tracks how many of the example crystals have been consumed
            player.GetModPlayer<MogPlayerStatIncrease>().aghanimShard++;
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrystalShard, 25).
                AddIngredient(ItemID.FallenStar, 15).
                AddIngredient(ItemID.Ectoplasm, 7).
                AddIngredient<UltimateOrb>(1).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}