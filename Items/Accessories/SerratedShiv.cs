using MogMod.Common.MogModPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class SerratedShiv : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.LightRed;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingSerratedShiv = true;

            player.GetDamage(DamageClass.Generic) += .10f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BladesOfAttack>(1).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Emblem"}", 1).
                AddIngredient(ItemID.TitaniumBar, 12).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
