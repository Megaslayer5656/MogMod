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
    public class MagicWand : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int FlatMagicDamageBoost = 3;
        public const int ManaBoost = 30;
        public const int LifeHeal = 7; // shared with mana
        public const int ManaHeal = 7; // shared with life
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FlatMagicDamageBoost, ManaBoost, LifeHeal);
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.WandKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic).Flat += FlatMagicDamageBoost;
            player.statManaMax2 += ManaBoost;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wandActive = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MagicStick>().
                AddIngredient<IronBranch>(2).
                AddIngredient<CraftingRecipe>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}