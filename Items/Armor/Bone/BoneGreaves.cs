using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Bone
{
    [AutoloadEquip(EquipType.Legs)]
    public class BoneGreaves : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 5;
            Item.rare = ItemRarityID.Green;
        }
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += .12f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Silk, 15).
                AddIngredient(ItemID.FossilOre, 15).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}