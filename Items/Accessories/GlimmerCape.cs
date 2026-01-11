using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
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
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Mod.Logger.Info("updated glimmer cape");
            // make a keybind to turn player invisible and grant movement speed for 1 minute with a 2 minute cooldown
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
                AddRecipeGroup("TissueSample", 7).
                AddIngredient(ItemID.FallenStar, 5).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
