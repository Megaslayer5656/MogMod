using Microsoft.Xna.Framework;
using MogMod.Buffs.AccessoryAuras;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Rarities;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems.Aspects
{
    public class MendingAspect : NeutralItem
    {
        public const int LifeRegen = 40;
        public const int LifeHeal = 50;
        public const float LifeMult = 0.3f;
        public static Color Colour = new(114f, 230f, 49f);
        public Color DescColor = new(114, 230, 49);
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
            Lighting.AddLight(Item.Center, 114f * brightness, 230f * brightness, 49f * brightness);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingMending = true;
            mogPlayer.mendingVisual = !hideVisual;

            int teamBuff = ModContent.BuffType<MendingRegenerationAura>();
            if (player.miscCounter % 10 == 0)
            {
                int myPlayer = Main.myPlayer;
                player.AddBuff(teamBuff, 20);
                if (Main.player[myPlayer].team == player.team && player.team != 0)
                {
                    float teamPlayerXDist = player.position.X - Main.player[myPlayer].position.X;
                    float teamPlayerYDist = player.position.Y - Main.player[myPlayer].position.Y;
                    if ((float)Math.Sqrt(teamPlayerXDist * teamPlayerXDist + teamPlayerYDist * teamPlayerYDist) < mogPlayer.auraRange)
                    {
                        Main.player[myPlayer].AddBuff(teamBuff, 20);
                    }
                }
            }
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
                    TooltipLine desc = new(Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, MiscUtils.GetTextFromModItem<MendingAspect>("Description").Format(LifeRegen.ToRegenPerSecond(), LifeMult.ToPercent()));
                    desc.OverrideColor = DescColor;
                    tooltips.Insert(index, desc);
                }
                else
                {
                    index++;
                    TooltipLine normal = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<MendingAspect>("AspectType").Format());
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