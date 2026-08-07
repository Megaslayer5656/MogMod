using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories.Boots;
using MogMod.Items.Accessories.NeutralItems.Aspects;
using MogMod.Items.Armor.Radiant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Utilities
{
    public static partial class ItemUtils
    {
        public static Item ActiveItem(this Player player) => Main.mouseItem.IsAir ? player.HeldItem : Main.mouseItem;
        public static bool CantUseHoldout(this Player player, bool needsToHold = true) => player == null || !player.active || player.dead || (!player.channel && needsToHold) || player.CCed || player.noItems;
        public static string TooltipHotkeyString(this ModKeybind mhk)
        {
            if (Main.dedServ || mhk is null)
                return "";

            List<string> keys = mhk.GetAssignedKeys();
            if (keys.Count == 0)
                return "NONE";
            else
            {
                StringBuilder sb = new StringBuilder(16);
                sb.Append(keys[0]);

                // In almost all cases, this code won't run, because there won't be multiple bindings for the hotkey. But just in case...
                for (int i = 1; i < keys.Count; ++i)
                    sb.Append(" / ").Append(keys[i]);
                return sb.ToString();
            }
        }
        /// <summary>
        /// Shortcut for finding a specific string in the tooltip and replacing it with a new string<br/>
        /// Typically used for dynamic tooltip updating. Consider overriding Tooltip or using String.Format for applying constants.
        /// </summary>
        /// <param name="tooltips">The tooltip list provided to a <b>ModifyTooltips</b> TML hook.</param>
        /// <param name="replacedKey">The key to be replaced.</param>
        /// <param name="replacedKey">The new key.</param>
        public static void FindAndReplace(this List<TooltipLine> tooltips, string replacedKey, string newKey)
        {
            TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Text.Contains(replacedKey));
            if (line != null)
                line.Text = line.Text.Replace(replacedKey, newKey);
        }
        /// <summary>
        /// Shortcut for finding all of a specific string in the tooltip and replacing it with a new string<br/>
        /// Typically used for dynamic tooltip updating. Consider overriding Tooltip or using String.Format for applying constants.
        /// </summary>
        /// <param name="tooltips">The tooltip list provided to a <b>ModifyTooltips</b> TML hook.</param>
        /// <param name="replacedKey">The key to be replaced.</param>
        /// <param name="replacedKey">The new key.</param>
        public static void FindAndReplaceAll(this List<TooltipLine> tooltips, string replacedKey, string newKey)
        {
            foreach (TooltipLine line in tooltips)
                line.Text = line.Text.Replace(replacedKey, newKey);
        }
        public static void IntegrateHotkey(this List<TooltipLine> tooltips, ModKeybind mhk)
        {
            if (Main.dedServ || mhk is null)
                return;

            string finalKey = mhk.TooltipHotkeyString();
            tooltips.FindAndReplace("[KEY]", finalKey);
        }
        public static bool InventoryHas(this Player player, params int[] items) => player.inventory.Any(item => items.Contains(item.type));
        public static bool PortableStorageHas(this Player player, params int[] items)
        {
            bool hasItem = false;
            if (player.bank.item.Any(item => items.Contains(item.type)))
                hasItem = true;
            if (player.bank2.item.Any(item => items.Contains(item.type)))
                hasItem = true;
            if (player.bank3.item.Any(item => items.Contains(item.type)))
                hasItem = true;
            if (player.bank4.item.Any(item => items.Contains(item.type)))
                hasItem = true;
            return hasItem;
        }
        public static Color BuffColor => new(255, 105, 237);
        public static Color TypelessDebuffColor => new(230, 202, 250);
        public static Color FreezingColor => new(143, 242, 255);
        public static Color ToxicColor => new(193, 134, 219);
        public static Color BlazingColor => new(232, 117, 39);
        public static Color GhostflameColor => new(204, 224, 221);
        public static Color DeathColor => new(94, 24, 24);
        public static Color WingsOfLightColor => new(255, 232, 163);
        public static Color SkadiColor => new(92, 87, 235);
        public static Color BleedColor => new(176, 5, 29);
        public static Color InfernoDebuffColor => new(245, 44, 44);
        public static Color InfernoDebuffColor2 => new(247, 194, 47);
        public static Color AghHexColor => new(34, 27, 194);
        public static Color AghHexColor2 => new(194, 27, 83);
        public static Color DivineDebuffColor => new(250, 231, 200);
        public static Color DivineDebuffColor2 => new(243, 200, 250);

        private static readonly Dictionary<int, List<(Color, float)>> debuffColorWeightsCache = [];

        public static Color GetDebuffTooltipNameColor(int debuffId)
        {
            var color = TypelessDebuffColor;

            if (debuffId == ModContent.BuffType<InfernoDebuff>())
                color = Color.Lerp(InfernoDebuffColor, InfernoDebuffColor2, (MathF.Sin(Main.GlobalTimeWrappedHourly * 2) + 1) / 4f);
            else if (debuffId == ModContent.BuffType<DivineMightDebuff>())
                color = Color.Lerp(DivineDebuffColor, DivineDebuffColor2, (MathF.Sin(Main.GlobalTimeWrappedHourly * 2) + 1) / 4f);
            else if (debuffId == ModContent.BuffType<AghanimHexDebuff>())
                color = Color.Lerp(AghHexColor, AghHexColor2, (MathF.Sin(Main.GlobalTimeWrappedHourly * 2) + 1) / 4f);
            else if (debuffId == ModContent.BuffType<FreezingDebuff>())
                color = FreezingColor;
            else if (debuffId == ModContent.BuffType<ToxicDebuff>())
                color = ToxicColor;
            else if (debuffId == ModContent.BuffType<BlazingDebuff>())
                color = BlazingColor;
            else if (debuffId == ModContent.BuffType<GhostflameDebuff>())
                color = GhostflameColor;
            else if (debuffId == ModContent.BuffType<WingsOfLightDebuff>())
                color = WingsOfLightColor;
            else if (debuffId == ModContent.BuffType<EyeOfSkadiDebuff>())
                color = SkadiColor;
            else if (debuffId == ModContent.BuffType<HeavyBleed>())
                color = BleedColor;
            else if (debuffId == ModContent.BuffType<BlackBladeDebuff>())
                color = DeathColor;

            // If this is actually a beneficial buff, color it as so
            else if (!Main.debuff[debuffId])
                color = BuffColor;

            return color;
        }
    }
}