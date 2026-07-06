using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Items.Weapons.Melee;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class Satanic : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.SatanicKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 50;
            // makes vampire knives crazy
            player.lifeSteal *= 2f;
            player.GetDamage(DamageClass.Generic) += .10f;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingSatanic = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Sange>(1).
                AddIngredient(ItemID.MoonStone, 1).
                AddIngredient<GriefBar>(10).
                AddIngredient(ItemID.BeetleHusk, 7).
                AddIngredient<VitalityBooster>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}