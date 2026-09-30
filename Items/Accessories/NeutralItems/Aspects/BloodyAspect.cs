using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Rarities;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace MogMod.Items.Accessories.NeutralItems.Aspects
{
    public class BloodyAspect : NeutralItem //TODO: Make this have a cool and unique effect
    {
        public const float DamageMult = 0.10f;
        public static Color Colour = new(255f, 84f, 24f);
        public Color DescColor = new(255, 8, 8);
        public const float BloodMult = 0.15f;
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
            Lighting.AddLight(Item.Center, 255f * brightness, 84f * brightness, 24f * brightness);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingBloody = true;
            mogPlayer.bloodyVisual = !hideVisual;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var neutralLine = new TooltipLine(Mod, "NeutralItem", "Neutral Item");
            tooltips.Insert(1, neutralLine);
            int index = tooltips.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
            string stats = string.Empty;
            if (index != -1)
            {
                if (Main.keyState.PressingShift())
                {
                    index++;
                    TooltipLine desc = new(Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, MiscUtils.GetTextFromModItem<BloodyAspect>("Description").Format(DamageMult.ToPercent()));
                    desc.OverrideColor = DescColor;
                    tooltips.Insert(index, desc);
                }
                else
                {
                    index++;
                    TooltipLine normal = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<BloodyAspect>("AspectType").Format());
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
