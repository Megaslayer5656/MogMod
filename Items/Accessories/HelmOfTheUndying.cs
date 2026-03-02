using MogMod.Common.MogModPlayer;
using MogMod.Items.Other;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class HelmOfTheUndying : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.Pink;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingUndyingHelm = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.EmpressFlightBooster, 1).
                AddIngredient<HelmOfIronWill>(1).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Grave"}", 10).
                AddIngredient<UltimateOrb>(3).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
