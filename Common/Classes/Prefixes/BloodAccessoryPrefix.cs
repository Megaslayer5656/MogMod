using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Common.Classes.Prefixes
{
    public class Brutal : BloodAccessoryPrefix
    {
        public override float bleedBuildup => 0.04f;
    }
    public class Gory : BloodAccessoryPrefix
    {
        public override float bleedBuildup => 0.03f;
    }
    public class Cruel : BloodAccessoryPrefix
    {
        public override float bleedBuildup => 0.02f;
    }
    public class Grim : BloodAccessoryPrefix
    {
        public override float bleedBuildup => 0.01f;
    }

    public abstract class BloodAccessoryPrefix : ModPrefix, ILocalizedModType
    {
        public new string LocalizationCategory => "Prefixes.Accessory";

        // Stats
        public virtual float bleedBuildup => 0f;

        // Prefix roll logic
        public override PrefixCategory Category => PrefixCategory.Accessory;
        public override bool CanRoll(Item item) => GetType() != typeof(BloodAccessoryPrefix);

        // Applying stealth generation
        public override void ApplyAccessoryEffects(Player player)
        {
            player.MogMod().bloodMult += bleedBuildup;
        }
        public override void ModifyValue(ref float valueMult)
        {
            float extraValue = 1f + (5f * bleedBuildup);
            valueMult *= extraValue;
            //valueMult = VanillaPrefixChange.RarityPlusOneButClosestToTierTwo;
        }
        // Extra tooltip for new modifier stats
        public LocalizedText BleedBuildupTooltip => MiscUtils.GetText($"{LocalizationCategory}.BleedBuildupTooltip");
        internal const string BleedBuildupTooltipID = "MogMod:PrefixBleedBuildup";
        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            yield return new TooltipLine(Mod, BleedBuildupTooltipID, BleedBuildupTooltip.Format((bleedBuildup * 100).ToString("N0")))
            {
                IsModifier = true
            };
        }
    }
}