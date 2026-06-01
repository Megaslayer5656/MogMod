using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class ForceStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.ForceStaffKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetKnockback(DamageClass.Generic) += .20f;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingForceStaff = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.EmeraldStaff, 1).
                AddIngredient(ItemID.PinkEskimoHood, 1).
                AddIngredient(ItemID.RichMahogany, 12).
                AddIngredient<RuntyBar>(8).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
