using Terraria.ModLoader;

namespace MogMod.Common.Classes
{
    public class SorceryDamageClass : DamageClass
    {
        internal static SorceryDamageClass Instance;
        public override void Load() => Instance = this;
        public override void Unload() => Instance = null;
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Magic || damageClass == Generic)
                return StatInheritanceData.Full;
            return StatInheritanceData.None;
        }
        public override bool GetEffectInheritance(DamageClass damageClass) => damageClass == Magic;
    }
}