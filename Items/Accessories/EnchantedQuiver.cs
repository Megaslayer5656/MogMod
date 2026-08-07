using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    [AutoloadEquip([EquipType.Back])]
    public class EnchantedQuiver : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const float VelocityMult = 1.3f;
        public override void SetDefaults()
        {
            Item.width = Item.height = 30;
            Item.accessory = true;
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingEnchantedQuiver = true;
            player.magicQuiver = true;
            player.arrowDamage += 0.15f;
            player.GetCritChance<RangedDamageClass>() += 10;
            player.GetAttackSpeed<RangedDamageClass>() += 0.10f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyQuiver").
                AddIngredient<FaeBar>(8).
                AddIngredient(ItemID.FragmentNebula, 8).
                AddIngredient<PointBooster>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}