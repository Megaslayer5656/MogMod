using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Fae
{
    [AutoloadEquip(EquipType.Legs)]
    public class FaeGreaves : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;

            Item.defense = 16;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Generic) += .12f;
            player.moveSpeed += .24f;
            player.maxMinions += 2;
            player.maxTurrets += 2;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FaeBar>(12).
                AddIngredient(ItemID.CrystalNinjaLeggings, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}