using Microsoft.Xna.Framework;
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
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingSatanic = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.LocalPlayer != null)
            {
                tooltips.FindAndReplace("[GFB]", this.GetLocalizedValue(Main.zenithWorld ? "TooltipGFB" : "TooltipDefault"));
                tooltips.IntegrateHotkey(KeybindSystem.SatanicKeybind);
            }
        }
        ModKeybind keybindActive = null;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Sange>().
                AddIngredient(ItemID.MoonStone).
                AddIngredient(ItemID.BeetleHusk, 7).
                AddIngredient<GriefBar>(5).
                AddIngredient<VitalityBooster>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}