using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.TankyRizzler
{
    [AutoloadEquip(EquipType.Legs)]
    public class TankyRizzlerLeggings : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 20;
            Item.rare = ItemRarityID.Cyan;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Melee) += .10f;
            player.moveSpeed += .1f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BeetleLeggings, 1).
                AddIngredient(ItemID.MartianConduitPlating, 75).
                AddIngredient<UltimateOrb>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}