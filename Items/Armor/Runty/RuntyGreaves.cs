using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Runty
{
    [AutoloadEquip(EquipType.Legs)]
    public class RuntyGreaves : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;

            Item.defense = 2;

            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += .08f;
            player.jumpSpeedBoost += 0.08f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RuntyBar>(12).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}