using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Rarities;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems.Aspects
{
    public class OverloadingAspect : NeutralItem
    {
        public const double DamageMult = 0.2D;
        public const int RegenWaitTime = 720;
        public const int EnemyRegenWaitTime = 360;
        public const int LifeRegenBoost = 60;
        public const int EnemyLifeRegenBoost = 180;
        public static Color Colour = new(49f, 174f, 230f);
        public Color DescColor = new(49, 174, 230);
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.rare = ModContent.RarityType<VonRarity>();
            Item.value = MogGlobalItem.RarityVonBuyPrice;
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.005f, 0.015f);
            Lighting.AddLight(Item.Center, 49f * brightness, 174f * brightness, 230f * brightness);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingOverloading = true;
            mogPlayer.overloadingVisual = !hideVisual;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var neutralLine = new TooltipLine(Mod, "NeutralItem", "Neutral Item"); // must manually be added for some slop reason
            tooltips.Insert(1, neutralLine);
            int index = tooltips.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
            string stats = string.Empty;
            if (index != -1)
            {
                if (Main.keyState.PressingShift())
                {
                    index++;
                    TooltipLine desc = new(Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, MiscUtils.GetTextFromModItem<OverloadingAspect>("Description").Format(DamageMult, RegenWaitTime.FramesToSeconds(), LifeRegenBoost.ToRegenPerSecond()));
                    desc.OverrideColor = DescColor;
                    tooltips.Insert(index, desc);
                }
                else
                {
                    index++;
                    TooltipLine normal = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<OverloadingAspect>("AspectType").Format());
                    tooltips.Insert(index, normal);
                    index++;
                    TooltipLine holdShiftIndicator = new(Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, MiscUtils.GetTextValue("UI.HoldShiftTooltipReplacementIndicator"));
                    holdShiftIndicator.OverrideColor = IHoldShiftTooltipItem.DefaultExtensionIndicatorColor;
                    tooltips.Insert(index, holdShiftIndicator);
                }
            }
        }
    }
}