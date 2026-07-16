using MogMod.Items.Global;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Common.Classes.Prefixes
{
    // TODO: add lots of prefixes (mega if you can think of any that are interesting feel free to add them)
    #region Positive
    // +damage, crit, attack speed, velocity, knockback && -mana cost
    public class Prestigious : SorceryDamagePrefix
    {
        public override float DamageMult => 1.1f;
        public override int CritBonus => 15;
        public override float UseTimeMult => 1.1f;
        public override float KnockbackMult => 1.1f;
        public override float ManaMult => 0.8f;
        public override float ShootSpeedMult => 1.1f;
    }
    // +attack speed && velocity
    public class Kinetic : SorceryDamagePrefix
    {
        public override float UseTimeMult => 1.15f;
        public override float ShootSpeedMult => 1.15f;
    }
    // -self damage
    public class Smooth : SorceryDamagePrefix
    {
        public override int StaffSelfHurtBonus => -3;
    }
    // +mana && crit
    public class Arcane : SorceryDamagePrefix
    {
        public override float ManaMult => 0.9f;
        public override int CritBonus => 3;
    }
    #endregion
    #region Positive && Negative
    // +damage && self damage
    public class Spiked : SorceryDamagePrefix
    {
        public override float DamageMult => 1.1f;
        public override int StaffSelfHurtBonus => 2;
    }
    // +damage && attack speed, but -knockback
    public class Savage : SorceryDamagePrefix
    {
        public override float DamageMult => 1.05f;
        public override float UseTimeMult => 1.05f;
        public override float KnockbackMult => 0.75f;
    }
    // +damage but -attack speed && velocity
    public class Latent : SorceryDamagePrefix
    {
        public override float DamageMult => 1.15f;
        public override float UseTimeMult => 0.9f;
        public override float ShootSpeedMult => 0.9f;
    }
    // +damage but +mana cost
    public class Demanding : SorceryDamagePrefix
    {
        public override float DamageMult => 1.2f;
        public override float ManaMult => 1.6f;
    }
    #endregion
    #region Negative
    // -damage && attack speed
    public class Dense : SorceryDamagePrefix
    {
        public override float DamageMult => 0.9f;
        public override float UseTimeMult => 0.9f;
    }
    // +mana cost && -velocity
    public class Dumb : SorceryDamagePrefix
    {
        public override float ManaMult => 1.2f;
        public override float ShootSpeedMult => 0.85f;
    }
    // +self damage
    public class Jagged : SorceryDamagePrefix
    {
        public override int StaffSelfHurtBonus => 4;
    }
    // evil
    public class Balrighted : SorceryDamagePrefix
    {
        public override float DamageMult => 0.7f;
        public override float UseTimeMult => 0.6f;
        public override float KnockbackMult => 0.7f;
        public override float ManaMult => 2f;
        public override float ShootSpeedMult => 0.6f;
        public override int StaffSelfHurtBonus => 10;
    }
    #endregion
    public abstract class SorceryDamagePrefix : ModPrefix, ILocalizedModType
    {
        public new string LocalizationCategory => "Prefixes.Weapon";
        /// <summary> The damage multiplier applied to spells. </summary>
        public virtual float DamageMult => 1f;
        /// <summary> The crit bonus added to spells. </summary>
        public virtual int CritBonus => 0;
        /// <summary> The mana cost multiplier applied to spells. </summary>
        public virtual float ManaMult => 1f;
        /// <summary> The attack speed multiplier applied to spells. </summary>
        public virtual float UseTimeMult => 1f;
        /// <summary> The velocity multiplier applied to spells. </summary>
        public virtual float ShootSpeedMult => 1f;
        /// <summary> The knockback multiplier applied to spells. </summary>
        public virtual float KnockbackMult => 1f;
        /// <summary> How much damage this item does to the player when a sorcery is casted. </summary>
        public virtual int StaffSelfHurtBonus => 0;
        public override PrefixCategory Category => PrefixCategory.AnyWeapon;
        public override bool CanRoll(Item item) => item.CountsAsClass<SorceryDamageClass>() && GetType() != typeof(SorceryDamagePrefix);

        // Applying normal weapon stats
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult = this.DamageMult;
            critBonus = this.CritBonus;
            useTimeMult = 1f;
        }
        public override void Apply(Item item)
        {
            if (item.CountsAsClass<SorceryDamageClass>() && item.TryGetGlobalItem<SorceryStaffGlobalItem>(out var sorceryStaff))
            {
                sorceryStaff.SelfHurtPrefixBonus = StaffSelfHurtBonus;
                sorceryStaff.AttackSpeedPrefixBonus = UseTimeMult;
                sorceryStaff.ManaCostPrefixBonus = ManaMult;
                sorceryStaff.VelocityPrefixBonus = ShootSpeedMult;
                sorceryStaff.KnockbackPrefixBonus = KnockbackMult;
            }
        }
        // Changing value based on prefix tier (rarity is set automatically around value multiplier)
        public override void ModifyValue(ref float valueMult)
        {
            float extra1 = StaffSelfHurtBonus;
            float extra2 = UseTimeMult - 1f;
            float extra3 = ManaMult - 1f;
            float extra4 = ShootSpeedMult - 1f;
            float extra5 = KnockbackMult - 1f;
            float valueMultiplier = 1f;
            float extraValue = 1f + valueMultiplier * (extra1 * extra2 * extra3 * extra4 * extra5);
            valueMult *= extraValue;
        }
        // Extra tooltip for new modifier stats
        public LocalizedText SelfHurtTooltip => MiscUtils.GetText($"{LocalizationCategory}.SorcerySelfDamageTooltip");
        internal const string SelfHurtTooltipID = "MogMod:PrefixSorceryDamage";
        public LocalizedText UseTimeTooltip => MiscUtils.GetText($"{LocalizationCategory}.SorceryUseTimeTooltip");
        internal const string UseTimeTooltipID = "MogMod:PrefixSorceryUseTime";
        public LocalizedText ManaTooltip => MiscUtils.GetText($"{LocalizationCategory}.SorceryManaTooltip");
        internal const string ManaTooltipID = "MogMod:PrefixSorceryMana";
        public LocalizedText VelocityTooltip => MiscUtils.GetText($"{LocalizationCategory}.SorceryVelocityTooltip");
        internal const string VelocityTooltipID = "MogMod:PrefixSorceryVelocity";
        public LocalizedText KnockbackTooltip => MiscUtils.GetText($"{LocalizationCategory}.SorceryKnockbackTooltip");
        internal const string KnockbackTooltipID = "MogMod:PrefixSorceryKnockback";
        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            if (StaffSelfHurtBonus != 0)
            {
                if (SelfHurtTooltip != null)
                    yield return new TooltipLine(Mod, SelfHurtTooltipID, SelfHurtTooltip.Format((StaffSelfHurtBonus >= 1f ? "+" : string.Empty) + StaffSelfHurtBonus.ToString("N0")))
                    {
                        IsModifier = true,
                        IsModifierBad = StaffSelfHurtBonus > 0
                    };
            }
            if (UseTimeMult != 1)
            {
                if (UseTimeTooltip != null)
                    yield return new TooltipLine(Mod, UseTimeTooltipID, UseTimeTooltip.Format((UseTimeMult >= 1f ? "+" : string.Empty) + ((UseTimeMult * 100) - 100).ToString("N0")))
                    {
                        IsModifier = true,
                        IsModifierBad = UseTimeMult < 1f
                    };
            }
            if (ManaMult != 1f)
            {
                if (ManaTooltip != null)
                    yield return new TooltipLine(Mod, ManaTooltipID, ManaTooltip.Format((ManaMult >= 1f ? "+" : string.Empty) + ((ManaMult * 100) - 100).ToString("N0")))
                    {
                        IsModifier = true,
                        IsModifierBad = ManaMult > 1f
                    };
            }
            if (ShootSpeedMult != 1f)
            {
                if (VelocityTooltip != null)
                    yield return new TooltipLine(Mod, VelocityTooltipID, VelocityTooltip.Format((ShootSpeedMult >= 1f ? "+" : string.Empty) + ((ShootSpeedMult * 100) - 100).ToString("N0")))
                    {
                        IsModifier = true,
                        IsModifierBad = ShootSpeedMult < 1f
                    };
            }
            if (KnockbackMult != 1f)
            {
                if (KnockbackTooltip != null)
                    yield return new TooltipLine(Mod, KnockbackTooltipID, KnockbackTooltip.Format((KnockbackMult >= 1f ? "+" : string.Empty) + ((KnockbackMult * 100) - 100).ToString("N0")))
                    {
                        IsModifier = true,
                        IsModifierBad = KnockbackMult < 1f
                    };
            }
            // Ignore this if there's no changes
            else
                yield break;
        }
    }
}