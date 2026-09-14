using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class HelmOfTheOverlord : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int MaxMinionsAndSentries = 2;
        public const float MagicAndSummonDamageBoost = 0.15f;
        public const int ManaBoost = 100;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxMinionsAndSentries, MagicAndSummonDamageBoost.ToPercent(), ManaBoost);
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += MagicAndSummonDamageBoost;
            player.GetDamage(DamageClass.Summon) += MagicAndSummonDamageBoost;
            player.statManaMax2 += ManaBoost;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.overlordMinion = true;
            mogPlayer.dominatorMinion = true;
            mogPlayer.diademMinion = true;
            mogPlayer.wearingHelmOfOverlord = true;
            player.UFOMinion = true; // temp slop
            player.spiderMinion = true; // temp slop
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HelmOfTheDominator>(1).
                AddIngredient<GriefBar>(7).
                AddIngredient<FaeBar>(7).
                AddIngredient<FuciumBar>(7).
                AddIngredient<ManaCore>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}