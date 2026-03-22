using MogMod.Common.MogModPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class ICBM : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Expert;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Generic) += .075f;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.atgActive = true;
            mogPlayer.icbmActive = true;
        }
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            if (equippedItem.type == ModContent.ItemType<PlasmaShrimp>() || equippedItem.type == ModContent.ItemType<ATGMissile>())
            {
                return false;
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
            AddIngredient<ATGMissile>(1).
            AddIngredient(ItemID.LunarBar, 15).
            AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Rocket"}", 50).
            AddTile(TileID.MythrilAnvil).
            Register();
        }
    }
}
