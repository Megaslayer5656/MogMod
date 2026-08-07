using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class ChaosDice : NeutralItem
    {
        public const int CritBoost = 17;
        public const int UltraCritChance = 10;
        public const float CritMult = 2.2f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBoost, UltraCritChance, CritMult);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 40;
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingChaosDice = true;
            player.GetCritChance<GenericDamageClass>() += CritBoost;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DestroyerEmblem).
                AddIngredient<GriefBar>(5).
                AddIngredient<CraftingRecipe>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}