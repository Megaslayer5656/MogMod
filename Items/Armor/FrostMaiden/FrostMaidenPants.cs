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
            Item.defense = 7;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 2;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Bone, 25).
                AddIngredient<FrigidShard>(5).
                AddIngredient(ItemID.FlinxFur, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}