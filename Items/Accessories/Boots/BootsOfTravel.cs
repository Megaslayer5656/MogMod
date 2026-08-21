using MogMod.Items.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.Boots
{
    [AutoloadEquip(EquipType.Shoes)]
    public class BootsOfTravel : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories.Boots";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 30;
            Item.height = 28;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.accRunSpeed = 10f;
            player.moveSpeed += .35f;
            player.noFallDmg = true; // Grants the player the Lucky Horseshoe effect of nullifying fall damage
            if (!hideVisual)
            {
                player.CancelAllBootRunVisualEffects(); // This ensures that boot visual effects don't overlap if multiple are equipped
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HermesBoots, 1).
                AddIngredient(ItemID.LuckyHorseshoe, 1).
                AddIngredient(ItemID.Aglet, 1).
                AddIngredient(ItemID.Bone, 30).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}