using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class IdeaRig : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;

            Item.accessory = true;
            
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingRigSlot = true;
        }
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            var rigs = MogGlobalItem.EnableNeutralItemSlots;
            if (rigs.Contains(incomingItem.type) && rigs.Contains(equippedItem.type))
                return false;
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Silk, 25).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Chest"}").
                AddTile(TileID.Loom).
                Register();
        }
    }
}