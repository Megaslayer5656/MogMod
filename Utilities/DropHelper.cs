using MogMod.NPCs.Global;
using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

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

        #region Lambda Drop Rule Condition
        // This class serves as a vanilla drop rule condition that is based on completely arbitrary code.
        // Create these using the function DropHelper.If as needed.
        internal class LambdaDropRuleCondition : IItemDropRuleCondition
        {
            private readonly Func<DropAttemptInfo, bool> conditionLambda;
            private readonly bool visibleInUI;
            private readonly string description;

            internal LambdaDropRuleCondition(Func<DropAttemptInfo, bool> lambda, bool ui = true, string desc = null)
            {
                conditionLambda = lambda;
                visibleInUI = ui;
                description = desc;
            }

            public bool CanDrop(DropAttemptInfo info) => conditionLambda(info);
            public bool CanShowItemDropInUI() => visibleInUI;
            public string GetConditionDescription() => description;
        }

        internal class LambdaDropRuleCondition2 : IItemDropRuleCondition
        {
            private readonly Func<DropAttemptInfo, bool> conditionLambda;
            private readonly Func<bool> visibleInUI;
            private readonly string description;

            internal LambdaDropRuleCondition2(Func<DropAttemptInfo, bool> lambda, Func<bool> ui, string desc = null)
            {
                conditionLambda = lambda;
                visibleInUI = ui;
                description = desc;
            }

            public bool CanDrop(DropAttemptInfo info) => conditionLambda(info);
            public bool CanShowItemDropInUI() => visibleInUI();
            public string GetConditionDescription() => description;
        }

        internal class LambdaDropRuleCondition3 : IItemDropRuleCondition
        {
            private readonly Func<DropAttemptInfo, bool> conditionLambda;
            private readonly Func<bool> visibleInUI;
            private readonly Func<string> description;

            internal LambdaDropRuleCondition3(Func<DropAttemptInfo, bool> lambda, Func<bool> ui, Func<string> desc)
            {
                conditionLambda = lambda;
                visibleInUI = ui;
                description = desc;
            }

            public bool CanDrop(DropAttemptInfo info) => conditionLambda(info);
            public bool CanShowItemDropInUI() => visibleInUI();
            public string GetConditionDescription() => description();
        }

        /// <summary>
        /// Creates a new LambdaDropRuleCondition which executes the code of your choosing to decide whether this item drop should occur.<br />
        /// This version of "If" does <b>NOT</b> use the DropAttemptInfo struct that is available.<br />
        /// This lets you write simpler lambdas that do not need the context, e.g. just checking if a boss is dead.
        /// </summary>
        /// <param name="lambda">Lambda function which evaluates to true or false, deciding whether the item should drop. <code>() => {CodeHere}</code></param>
        /// <returns>The LambdaDropRuleCondition produced.</returns>
        public static IItemDropRuleCondition If(Func<bool> lambda) => new LambdaDropRuleCondition((_) => lambda());

        /// <summary>
        /// Creates a new LambdaDropRuleCondition which executes the code of your choosing to decide whether this item drop should occur.<br />
        /// This version of "If" does <b>NOT</b> use the DropAttemptInfo struct that is available.<br />
        /// This lets you write simpler lambdas that do not need the context, e.g. just checking if a boss is dead.
        /// </summary>
        /// <param name="lambda">Lambda function which evaluates to true or false, deciding whether the item should drop. <code>() => {CodeHere}</code></param>
        /// <param name="ui">Whether drops registered with this condition appear in the Bestiary. Defaults to true.</param>
        /// <param name="desc">The description of this condition in the Bestiary. Defaults to null.</param>
        /// <returns>The LambdaDropRuleCondition produced.</returns>
        public static IItemDropRuleCondition If(Func<bool> lambda, bool ui = true, string desc = null)
        {
            bool LambdaInfoWrapper(DropAttemptInfo _) => lambda();
            return new LambdaDropRuleCondition(LambdaInfoWrapper, ui, desc);
        }
        public static IItemDropRuleCondition If(Func<bool> lambda, Func<bool> ui, string desc = null)
        {
            bool LambdaInfoWrapper(DropAttemptInfo _) => lambda();
            return new LambdaDropRuleCondition2(LambdaInfoWrapper, ui, desc);
        }
        public static IItemDropRuleCondition If(Func<bool> lambda, Func<bool> ui, Func<string> desc)
        {
            bool LambdaInfoWrapper(DropAttemptInfo _) => lambda();
            return new LambdaDropRuleCondition3(LambdaInfoWrapper, ui, desc);
        }

        /// <summary>
        /// Creates a new LambdaDropRuleCondition which executes the code of your choosing to decide whether this item drop should occur.<br />
        /// This version of "If" <b>DOES</b> use the DropAttemptInfo struct, and thus the provided lambda requires 1 argument.
        /// </summary>
        /// <param name="lambda">Lambda function which evaluates to true or false, deciding whether the item should drop. <code>(info) => {CodeHere}</code></param>
        /// <returns>The LambdaDropRuleCondition produced.</returns>
        public static IItemDropRuleCondition If(Func<DropAttemptInfo, bool> lambda) => new LambdaDropRuleCondition(lambda);

        /// <summary>
        /// Creates a new LambdaDropRuleCondition which executes the code of your choosing to decide whether this item drop should occur.<br />
        /// This version of "If" <b>DOES</b> use the DropAttemptInfo struct, and thus the provided lambda requires 1 argument.
        /// </summary>
        /// <param name="lambda">Lambda function which evaluates to true or false, deciding whether the item should drop. <code>(info) => {CodeHere}</code></param>
        /// <param name="ui">Whether drops registered with this condition appear in the Bestiary. Defaults to true.</param>
        /// <param name="desc">The description of this condition in the Bestiary. Defaults to null.</param>
        /// <returns>The LambdaDropRuleCondition produced.</returns>
        public static IItemDropRuleCondition If(Func<DropAttemptInfo, bool> lambda, bool ui = true, string desc = null)
        {
            return new LambdaDropRuleCondition(lambda, ui, desc);
        }
        public static IItemDropRuleCondition If(Func<DropAttemptInfo, bool> lambda, Func<bool> ui, string desc = null)
        {
            return new LambdaDropRuleCondition2(lambda, ui, desc);
        }
        public static IItemDropRuleCondition If(Func<DropAttemptInfo, bool> lambda, Func<bool> ui, Func<string> desc)
        {
            return new LambdaDropRuleCondition3(lambda, ui, desc);
        }
        #endregion
        public static IItemDropRuleCondition PostEye(bool ui = true) => Condition.DownedEyeOfCthulhu.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostEvil(bool ui = true) => Condition.DownedEowOrBoc.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostSkele(bool ui = true) => Condition.DownedSkeletron.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostOneMech(bool ui = true) => Condition.DownedMechBossAny.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostAllMech(bool ui = true) => Condition.DownedMechBossAll.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostPlant(bool ui = true) => Condition.DownedPlantera.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostFish(bool ui = true) => Condition.DownedDukeFishron.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostEoL(bool ui = true) => Condition.DownedEmpressOfLight.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);


        public static IItemDropRuleCondition OverloadingEliteCondition = If((info) =>
        {
            NPC npc = info.npc;
            return npc.MogMod().overloadingElite;
        });
        public static IItemDropRuleCondition BlazingEliteCondition = If((info) =>
        {
            NPC npc = info.npc;
            return npc.MogMod().fireElite;
        });
        public static IItemDropRuleCondition GildedEliteCondition = If((info) =>
        {
            NPC npc = info.npc;
            return npc.MogMod().goldElite;
        });
        public static IItemDropRuleCondition MendingEliteCondition = If((info) =>
        {
            NPC npc = info.npc;
            return npc.MogMod().healingElite;
        });
        public static IItemDropRuleCondition ToxicEliteCondition = If((info) =>
        {
            NPC npc = info.npc;
            return npc.MogMod().toxicElite;
        });

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
