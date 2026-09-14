using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class HelmOfTheDominator : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int MaxMinionsAndSentries = 2;
        public const float MagicAndSummonDamageBoost = 0.1f;
        public const int ManaBoost = 50;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxMinionsAndSentries, MagicAndSummonDamageBoost.ToPercent(), ManaBoost);
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += MagicAndSummonDamageBoost;
            player.GetDamage(DamageClass.Summon) += MagicAndSummonDamageBoost;
            player.statManaMax2 += ManaBoost;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.dominatorMinion = true;
            mogPlayer.diademMinion = true;
            mogPlayer.wearingHelmOfDominator = true;
            player.spiderMinion = true; // temp slop
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HelmOfIronWill>().
                AddIngredient<Diadem>().
                AddRecipeGroup("AnyEmblem").
                AddRecipeGroup("AnyCobaltBar", 8).
                AddIngredient(ItemID.Topaz, 2).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}