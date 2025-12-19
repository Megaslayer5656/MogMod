using log4net.Repository.Hierarchy;
using MogMod.Common.Player;
using MogMod.Common.Systems;
using MogMod.Items.Other;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            player.GetDamage(DamageClass.Magic) += .07f;
            player.GetDamage(DamageClass.Summon) += .07f;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.isWearingGlimmerCape = true;
            
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Robe, 1).
                AddIngredient<ShadowAmulet>(1).
                AddIngredient(ItemID.ShadowScale, 7 ).
                AddIngredient(ItemID.FallenStar, 5).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.Robe, 1).
                AddIngredient<ShadowAmulet>(1).
                AddIngredient(ItemID.TissueSample, 7).
                AddIngredient(ItemID.FallenStar, 5).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
