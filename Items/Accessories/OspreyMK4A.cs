using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class OspreyMK4A : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;

            Item.accessory = true;
            Item.defense = 8;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingRigSlot = true;
            mogPlayer.ammoCost *= 0.85f;
            player.endurance += 0.05f;
            player.GetCritChance<RangedDamageClass>() += 12;
            player.moveSpeed -= 0.15f;
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
                AddIngredient<ScavVest>().
                AddIngredient(ItemID.LunarTabletFragment, 8).
                AddIngredient<FrostEssence>(5).
                AddIngredient<SpookyEssence>(5).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}