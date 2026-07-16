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
    }
}