using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class HelmOfTheUndying : NeutralItem
    {
        public const float RespawnMult = 0.2f;
        public const double GFBRespawnMult = 5D;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingUndyingHelm = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var neutralLine = new TooltipLine(Mod, "NeutralItem", "Neutral Item");
            tooltips.Insert(1, neutralLine);
            int index = tooltips.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (index != -1)
            {
                if (Main.zenithWorld)
                {
                    index++;
                    TooltipLine gfb = new(Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, MiscUtils.GetTextFromModItem<HelmOfTheUndying>("TooltipGFB").Format(GFBRespawnMult));
                    tooltips.Insert(index, gfb);
                }
                else
                {
                    index++;
                    TooltipLine normal = new(Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, MiscUtils.GetTextFromModItem<HelmOfTheUndying>("TooltipNormal").Format(RespawnMult.ToPercent()));
                    tooltips.Insert(index, normal);
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HelmOfIronWill>().
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Grave"}", 10).
                AddIngredient<UltimateOrb>(3).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}