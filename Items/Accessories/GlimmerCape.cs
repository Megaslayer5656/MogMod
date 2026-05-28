using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class GlimmerCape : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.GlimmerCapeKeybind);
        ModKeybind keybindActive = null;

        public override void Load()
        {
            //base.Load();
        }

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
            player.statManaMax2 += 50;
            player.aggro -= 600;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.isWearingGlimmerCape = true;
            
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Robe, 1).
                AddIngredient<ShadowAmulet>(1).
                AddIngredient<FrigidShard>(3).
                AddIngredient<ManaEssence>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
