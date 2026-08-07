using MogMod.Items.Global;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class HelmOfIronWill : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
            Item.defense = 3;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.lifeRegen += 2;
            player.statLifeMax2 += 20;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("IronBar", 20).
                AddRecipeGroup("AnyGoldBar", 15).
                AddRecipeGroup("AnySilverBar", 12).
                AddIngredient(ItemID.Diamond, 6).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
