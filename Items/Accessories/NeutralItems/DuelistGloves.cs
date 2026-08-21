using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    [AutoloadEquip(EquipType.HandsOn)]
    public class DuelistGloves : NeutralItem
    {
        public const float AttackSpeedBoost = 0.07f;
        public const int MaxAttackSpeedBoost = 3;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AttackSpeedBoost.ToPercent(), MaxAttackSpeedBoost * (AttackSpeedBoost * 100));
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 20;
            Item.height = 22;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingDuelistGloves = true;
            player.autoReuseGlove = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FeralClaws, 1).
                AddIngredient(ItemID.Cactus, 75).
                AddIngredient<RuntyBar>(8).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}