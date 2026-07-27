using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class GunpowderGauntlet : NeutralItem
    {
        public static double DamageMult = 1.5D;
        public const int DamageCap = 80;
        public const float MagicSpeedBoost = 0.05f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MagicSpeedBoost.ToPercent(), DamageMult, DamageCap);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingGunpowderGauntlet = true;

            player.GetAttackSpeed(DamageClass.Magic) += MagicSpeedBoost;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Dynamite, 12).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Flesh"}", 8).
                AddIngredient(ItemID.Leather, 5).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}