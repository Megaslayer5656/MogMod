using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace MogMod.Utilities
{
    public struct Fraction
    {
        internal readonly int numerator;
        internal readonly int denominator;

        public Fraction(int n, int d)
        {
            numerator = n < 0 ? 0 : n;
            denominator = d <= 0 ? 1 : d;
        }

        public static implicit operator float(Fraction f) => f.numerator / (float)f.denominator;
    }
    public static class DropHelper
    {
        public static LeadingConditionRule DefineConditionalDropSet(this ILoot loot, IItemDropRuleCondition condition)
        {
            LeadingConditionRule rule = new LeadingConditionRule(condition);
            loot.Add(rule);
            return rule;
        }
        /// <summary>
        /// Shorthand for shorthand: Registers an item to drop per-player on the specified condition.<br />
        /// Intended for lore items, but can be used generally for instanced drops.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="lambda">A lambda which evaluates in real-time to the condition that needs to be checked.</param>
        /// <param name="itemID">The item ID to drop.</param>
        /// <param name="ui">Whether drops registered with this condition appear in the Bestiary. Defaults to true.</param>
        /// <param name="desc">The description of this condition in the Bestiary. Defaults to null.</param>
        /// <returns>A LeadingConditionRule which you can attach more PerPlayer or other rules to as you want.</returns>
        
        //public static LeadingConditionRule AddConditionalPerPlayer(this ILoot loot, Func<bool> lambda, int itemID, bool ui = true, string desc = null)
        //{
        //    LeadingConditionRule lcr = new(If(lambda, ui, desc));
        //    lcr.Add(PerPlayer(itemID));
        //    loot.Add(lcr);
        //    return lcr;
        //}
        public static IItemDropRule AddNormalOnly(this ILoot loot, int itemID, Fraction dropRate, int minQuantity = 1, int maxQuantity = 1)
        {
            return loot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), itemID, dropRate.denominator, minQuantity, maxQuantity, dropRate.numerator));
        }

        /// <summary>
        /// Shorthand to add an arbitrary drop rule as a normal-only drop to a loot table.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="rule">The IItemDropRule to add.</param>
        
        //public static void AddNormalOnly(this ILoot loot, IItemDropRule rule)
        //{
        //    LeadingConditionRule normalOnly = loot.DefineNormalOnlyDropSet();
        //    normalOnly.Add(rule);
        //}

        /// <summary>
        /// Shorthand for shorthand: Registers a Normal Mode only LeadingConditionRule for a loot table and returns it to you.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <returns>A Normal Mode only LeadingConditionRule.</returns>
        public static LeadingConditionRule DefineNormalOnlyDropSet(this ILoot loot) => loot.DefineConditionalDropSet(new Conditions.NotExpert());

        public class PerPlayerDropRule : CommonDrop
        {
            /// Default instanced drops are protected for 15 minutes, because they are used for boss bags.
            /// You can customize this duration as you see fit. Calamity defaults it to 5 minutes.
            private const int DefaultDropProtectionTime = 18000; // 5 minutes
            private int protectionTime;

            public PerPlayerDropRule(int itemID, int denominator, int minQuantity = 1, int maxQuantity = 1, int numerator = 1, int protectFrames = DefaultDropProtectionTime)
                : base(itemID, denominator, minQuantity, maxQuantity, numerator)
            {
                protectionTime = protectFrames;
            }

            public PerPlayerDropRule(int itemID, Fraction dropRate, int minQuantity = 1, int maxQuantity = 1)
                : base(itemID, dropRate.denominator, minQuantity, maxQuantity, dropRate.numerator)
            {
                protectionTime = DefaultDropProtectionTime;
            }

            /// Overriding CanDrop is unnecessary. This drop rule has no condition.
            /// If you want to use a condition with PerPlayerDropRule, use DropHelper.If

            //public override ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
            //{
            //    ItemDropAttemptResult result = default;
            //    if (info.rng.Next(chanceDenominator) < chanceNumerator)
            //    {
            //        int stack = info.rng.Next(amountDroppedMinimum, amountDroppedMaximum + 1);
            //        TryDropInternal(info, itemId, stack);
            //        result.State = ItemDropAttemptResultState.Success;
            //        return result;
            //    }

            //    result.State = ItemDropAttemptResultState.FailedRandomRoll;
            //    return result;
            //}

            /// The contents of this method are more or less copied from CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0

            //private void TryDropInternal(DropAttemptInfo info, int itemId, int stack)
            //{
            //    if (itemId <= 0 || itemId >= ItemLoader.ItemCount)
            //        return;

            //    // If server-side, then the item must be spawned for each client individually.
            //    if (Main.netMode == NetmodeID.Server)
            //    {
            //        NPC npc = info.npc;
            //        int idx = Item.NewItem(npc.GetSource_Loot(), npc.Center, itemId, stack, true, -1);
            //        Main.timeItemSlotCannotBeReusedFor[idx] = protectionTime;
            //        foreach (Player player in Main.ActivePlayers)
            //        {
            //            NetMessage.SendData(MessageID.InstancedItem, player.whoAmI, -1, null, idx);
            //        }

            //        Main.item[idx].active = false;
            //    }

            //    // Otherwise just drop the item.
            //    else
            //        CommonCode.DropItem(info, itemId, stack);
            //}
        }
    }
}
