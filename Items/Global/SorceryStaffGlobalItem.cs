using MogMod.Common.Classes;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Items.Global
{
    public sealed class SorceryStaffGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        /// <summary> Tracks the self damage inflicted on the player modifier for this item, derived from its prefix. </summary>
        internal int SelfHurtPrefixBonus = 0;
        /// <summary> Tracks the attack speed modifier for this item, derived from its prefix. </summary>
        internal float AttackSpeedPrefixBonus = 1f;
        /// <summary> Tracks the mana cost modifier for this item, derived from its prefix. </summary>
        internal float ManaCostPrefixBonus = 1f;
        /// <summary> Tracks the velocity modifier for this item, derived from its prefix. </summary>
        internal float VelocityPrefixBonus = 1f;
        /// <summary> Tracks the knockback modifier for this item, derived from its prefix. </summary>
        internal float KnockbackPrefixBonus = 1f;
        public override GlobalItem Clone(Item from, Item to)
        {
            SorceryStaffGlobalItem myClone = (SorceryStaffGlobalItem)base.Clone(from, to);

            myClone.SelfHurtPrefixBonus = SelfHurtPrefixBonus;
            myClone.AttackSpeedPrefixBonus = AttackSpeedPrefixBonus;
            myClone.ManaCostPrefixBonus = ManaCostPrefixBonus;
            myClone.VelocityPrefixBonus = VelocityPrefixBonus;
            myClone.KnockbackPrefixBonus = KnockbackPrefixBonus;

            return myClone;
        }
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.CountsAsClass<SorceryDamageClass>();
        }
        public override void PreReforge(Item item)
        {
            SelfHurtPrefixBonus = 0;
            AttackSpeedPrefixBonus = 1f;
            ManaCostPrefixBonus = 1f;
            VelocityPrefixBonus = 1f;
            KnockbackPrefixBonus = 1f;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.prefix > 0) tooltips.RemoveAll(line => line.Name == "PrefixSpeed");
        }
    }
}