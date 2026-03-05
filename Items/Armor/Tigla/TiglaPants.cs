using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Tigla
{
    [AutoloadEquip(EquipType.Legs)]
    public class TiglaPants : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 22;
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Ranged) += 9;
            player.jumpSpeedBoost += 0.15f;
            player.moveSpeed += 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ShroomiteLeggings, 1).
                AddIngredient(ItemID.Cog, 75).
                AddIngredient<UltimateOrb>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}