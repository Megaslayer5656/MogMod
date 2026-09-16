using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class SerratedShiv : NeutralItem
    {
        public const float ProcChance = 0.08f;
        public const float MaxLifeDamage = 0.005f;
        public const int DamageCap = 400;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ProcChance.ToPercent(), MaxLifeDamage.ToPercent(), DamageCap);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = MogGlobalItem.RarityLightPurpleBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingSerratedShiv = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BladesOfAttack>().
                AddRecipeGroup("AnyEmblem").
                AddRecipeGroup("AnyAdamantiteBar", 18).
                AddIngredient<FuciumBar>(12).
                AddIngredient(ItemID.SoulofFright, 7).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}