using Terraria.ModLoader;

namespace MogMod.Common.Classes
{
    public class MeleeRangedDamageClass : DamageClass
    {
        internal static MeleeRangedDamageClass Instance;
        internal static readonly StatInheritanceData FiftyPercentBoost = new(0.5f, 0.5f, 0.5f, 0.5f, 0.5f);
        public override void Load() => Instance = this;
        public override void Unload() => Instance = null;
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Melee || damageClass == Ranged)
                return FiftyPercentBoost;
            if (damageClass == Generic)
                return StatInheritanceData.Full;
            return StatInheritanceData.None;
        }
        public override bool GetEffectInheritance(DamageClass damageClass) => damageClass == Melee || damageClass == Ranged;
    }
}