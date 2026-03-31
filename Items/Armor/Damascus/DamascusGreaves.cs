using MogMod.Items.Other;
using MogMod.Items.Placeable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Damascus
{
    [AutoloadEquip(EquipType.Legs)]
    public class DamascusGreaves : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 16;
            Item.defense = 6;
            Item.rare = ItemRarityID.LightRed;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<GenericDamageClass>() += 8;
            player.moveSpeed += .10f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FuciumBar>(10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}