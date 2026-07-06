using MogMod.Items.Global;
using MogMod.Items.Other;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class Vanguard : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
            Item.defense = 5;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 50;
            player.lifeRegen += 4;
            player.noKnockback = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CobaltShield, 1).
                AddIngredient(ItemID.BandofRegeneration, 1).
                AddIngredient(ItemID.HellstoneBar, 12).
                AddIngredient<VitalityBooster>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
