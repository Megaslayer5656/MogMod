using Microsoft.Xna.Framework;
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

namespace MogMod.Items.Accessories.Rigs
{
    public class AzimutSSZhuk : ChestRig, IHoldShiftTooltipItem
    {
        public const float MiningSpeedBoost = 0.25f;
        //public const int MagSize = 50;
        //public const int MagReload = 50;
        public bool HidesNormalTooltip => true;
        public Color? TooltipExtensionColor => new(170, 170, 170);
        //public LocalizedText TooltipExtensionText => this.GetLocalization("HoldShiftTooltip").WithFormatArgs(MagSize, MagReload.FramesToSeconds());
        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.width = 30;
            Item.height = 26;

            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingZhuk = true;
            //mogPlayer.maxShots = MagSize;
            //mogPlayer.reloadTime = MagReload;
            player.pickSpeed -= Main.zenithWorld ? -MiningSpeedBoost : MiningSpeedBoost;

            int chestNumb = Main.zenithWorld ? -4 : -3;
            if (mogPlayer.zhukActive && player.chest != chestNumb)
                player.chest = chestNumb;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if (Main.zenithWorld)
                return Color.Black;
            return lightColor;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var rigLine = new TooltipLine(Mod, "RigAccessory", "Chest Rig");
            tooltips.Insert(1, rigLine);
            var Hotkey = KeybindSystem.RigKeybind.TooltipHotkeyString();
            int index = tooltips.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
            string stats = string.Empty;
            if (index != -1)
            {
                if (Main.zenithWorld)
                {
                    index++;
                    TooltipLine life = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<AzimutSSZhuk>("TooltipGFB").Format(
                    MiningSpeedBoost.ToPercent(), // {0}
                    Hotkey)); // {1}
                    life.OverrideColor = Main.DiscoColor;
                    tooltips.Insert(index, life);
                }
                else
                {
                    index++;
                    TooltipLine life = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<AzimutSSZhuk>("TooltipNormal").Format(
                    MiningSpeedBoost.ToPercent(), // {0}
                    Hotkey)); // {1}
                    tooltips.Insert(index, life);
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TritonM43A>().
                AddIngredient<UltimateOrb>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}