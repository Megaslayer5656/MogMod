using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.FrostMaiden
{
    [AutoloadEquip(EquipType.Legs)]
    public class FrostMaidenPants : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 18;
            Item.defense = 3;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += 20;
            player.moveSpeed += .12f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FrigidShard>(4).
                AddIngredient<ManaEssence>(2).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}