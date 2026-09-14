using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class HeartOfTarrasque : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const float PotionReduction = 0.9f;
        public const int LifeRegenCap = 80;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(((1 - PotionReduction) + (1 - Player.PhilosopherStoneDurationMultiplier)).ToPercent(), LifeRegenCap.ToRegenPerSecond());
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
            Item.defense = 20;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            int lifeLeft = player.statLifeMax2 - player.statLife;
            int lifeRegen = (int)(lifeLeft * 0.15f) + 5;
            if (lifeRegen > LifeRegenCap) lifeRegen = LifeRegenCap;
            player.lifeRegen += lifeRegen;
            player.shinyStone = true;
            player.PotionDelayModifier *= PotionReduction;
            player.pStone = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ShinyStone).
                AddIngredient(ItemID.CharmofMyths).
                AddTile(TileID.TinkerersWorkbench).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.CharmofMyths).
                AddIngredient<LizhardBloodVial>().
                AddIngredient<SoulOfMogMod>().
                AddIngredient<UltimateOrb>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}