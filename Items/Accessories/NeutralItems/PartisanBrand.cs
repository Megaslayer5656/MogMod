using MogMod.Common.Classes;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class PartisanBrand : NeutralItem
    {
        public const float SorceryAndSummonDamageBoost = 0.07f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SorceryAndSummonDamageBoost.ToPercent());
        public override void SetStaticDefaults() => ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<WhisperOfTheDread>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 28;
            Item.height = 34;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<SorceryDamageClass>() += SorceryAndSummonDamageBoost;
            player.GetDamage<SummonDamageClass>() += SorceryAndSummonDamageBoost;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                DisableDecraft().
                AddIngredient(ItemID.AshWood, 40).
                AddIngredient(ItemID.HellstoneBar, 8).
                AddIngredient<PointBooster>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}