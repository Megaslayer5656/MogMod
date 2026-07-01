using MogMod.Buffs.Debuffs;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Common.Systems
{
    [ReinitializeDuringResizeArrays]
    public static class MogModBuffSets
    {
        private static SetFactory Factory = BuffID.Sets.Factory;
        /// <summary>
        /// If <see langword="true"/> for a buff type, then that buff is considered to be a debuff.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] IsDebuff = Factory.CreateNamedSet("IsDebuff")
            .Description("General-purpose set with several different uses")
            .RegisterBoolSet(BuffID.Poisoned, BuffID.Darkness, BuffID.Cursed, BuffID.OnFire, BuffID.Bleeding, BuffID.Confused, BuffID.Slow, BuffID.Weak, BuffID.Silenced, BuffID.BrokenArmor,
                BuffID.CursedInferno, BuffID.Frostburn, BuffID.Chilled, BuffID.Frozen, BuffID.Burning, BuffID.Suffocation, BuffID.Ichor, BuffID.Venom, BuffID.Blackout, BuffID.Electrified,
                BuffID.Rabies, BuffID.Webbed, BuffID.Stoned, BuffID.Dazed, BuffID.VortexDebuff, BuffID.WitheredArmor, BuffID.WitheredWeapon, BuffID.ShadowFlame, BuffID.OgreSpit, BuffID.BetsysCurse,
                BuffID.Wet, BuffID.Slimed, BuffID.OnFire3, BuffID.Frostburn2, BuffType<GhostflameDebuff>(), BuffType<GlueDebuff>(), BuffType<GreenTracerDebuff>(), BuffType<InfernoDebuff>(),
                BuffType<JidiPollenBagDebuff>(), BuffType<KrakenShellDebuff>(), BuffType<ShivasEnemyDebuff>(), BuffType<VonDebuff>(), BuffType<WingsOfLightDebuff>());
    }
}
