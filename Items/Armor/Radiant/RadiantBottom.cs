using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Radiant
{
    [AutoloadEquip(EquipType.Legs)]
    public class RadiantBottom : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 26;
            Item.defense = 11;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MagicDamageClass>() += 0.1f;
            player.jumpSpeedBoost += 0.1f;
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpectrePants, 1).
                AddIngredient<FrigidCrystal>(3).
                AddIngredient<ManaCore>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}