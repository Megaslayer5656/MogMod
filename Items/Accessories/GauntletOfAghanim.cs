using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    [AutoloadEquip([EquipType.HandsOn, EquipType.HandsOff])]
    public class GauntletOfAghanim : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingAghGauntlet = true;
            player.kbGlove = true;
            player.autoReuseGlove = true;
            player.meleeScaleGlove = true;
            mogPlayer.gloveLevel = 5;
            player.GetDamage<MeleeDamageClass>() += 0.10f;
            mogPlayer.aghGauntletVisual = !hideVisual;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FireGauntlet).
                AddIngredient<VoniumBar>(5).
                AddIngredient<SoulFragment>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}