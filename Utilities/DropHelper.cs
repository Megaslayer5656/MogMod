using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;
using Terraria;

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
        public static IItemDropRuleCondition PostEvil(bool ui = true) => Condition.DownedEowOrBoc.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostFish(bool ui = true) => Condition.DownedDukeFishron.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostEoL(bool ui = true) => Condition.DownedEmpressOfLight.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostOneMech(bool ui = true) => Condition.DownedMechBossAny.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostAllMech(bool ui = true) => Condition.DownedMechBossAll.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRule Add(this LeadingConditionRule mainRule, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1, bool hideLootReport = false)
        {
            return mainRule.OnSuccess(ItemDropRule.Common(itemID, dropRateInt, minQuantity, maxQuantity), hideLootReport);
        }
        public static LeadingConditionRule DefineConditionalDropSet(this ILoot loot, IItemDropRuleCondition condition)
        {
            LeadingConditionRule rule = new LeadingConditionRule(condition);
            loot.Add(rule);
            return rule;
        }
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
        }
    }
}
