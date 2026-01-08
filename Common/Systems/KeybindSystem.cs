using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace MogMod.Common.Systems
{
    public class KeybindSystem : ModSystem
    {
        #region Keybind Setup
        public static ModKeybind GlimmerCapeKeybind { get; private set; }
        public static ModKeybind ArmletKeybind { get; private set; }
        public static ModKeybind SatanicKeybind { get; private set; }
        public static ModKeybind ArcaneBootsKeybind { get; private set; }
        public static ModKeybind RefresherOrbKeybind { get; private set; }
        public static ModKeybind WandKeybind { get; private set; }
        public static ModKeybind HelmOfDominatorKeybind { get; private set; }
        public static ModKeybind GuardianGreavesKeybind { get; private set; }
        public static ModKeybind MekansmKeybind { get; private set; }
        public static ModKeybind ForceStaffKeybind { get; private set; }
        public static ModKeybind BladeMailKeybind { get; private set; }
        public static ModKeybind ShivasKeybind {  get; private set; }
        #endregion

        public override void Load()
        {
            // Registers a new keybind
            // We localize keybinds by adding a Mods.{ModName}.Keybind.{KeybindName} entry to our localization files. The actual text displayed to English users is in en-US.hjson
            #region Healing/Mana
            RefresherOrbKeybind = KeybindLoader.RegisterKeybind(Mod, "ActivateRefresherOrb", "V");
            ArcaneBootsKeybind = KeybindLoader.RegisterKeybind(Mod, "ActivateManaBoots", "C");
            GuardianGreavesKeybind = ArcaneBootsKeybind;
            WandKeybind = KeybindLoader.RegisterKeybind(Mod, "ActivateMagicWand", "Z");
            MekansmKeybind = KeybindLoader.RegisterKeybind(Mod, "ActivateMekansm", "C");
            #endregion

            #region Defensive/Mobility
            GlimmerCapeKeybind = KeybindLoader.RegisterKeybind(Mod, "GlimmerCape", "X");
            SatanicKeybind = KeybindLoader.RegisterKeybind(Mod, "ActivateSatanic", "X");
            ForceStaffKeybind = KeybindLoader.RegisterKeybind(Mod, "ActivateForceStaff", "F");
            HelmOfDominatorKeybind = KeybindLoader.RegisterKeybind(Mod, "ActivateHelmOfTheDominator", "G");
            #endregion

            #region Attack
            ShivasKeybind = KeybindLoader.RegisterKeybind(Mod, "ActivateShiva'sGuard", "C");
            BladeMailKeybind = KeybindLoader.RegisterKeybind(Mod, "ActivateBladeMail", "X");
            ArmletKeybind = KeybindLoader.RegisterKeybind(Mod, "ToggleArmletOfMordiggian", "T");
            #endregion
        }

        public override void Unload()
        {
            #region Keybind Reset
            // Not required if your AssemblyLoadContext is unloading properly, but nulling out static fields can help you figure out what's keeping it loaded.
            GlimmerCapeKeybind = null;
            ArmletKeybind = null;
            SatanicKeybind = null;
            ArcaneBootsKeybind = null;
            RefresherOrbKeybind = null;
            WandKeybind = null;
            HelmOfDominatorKeybind = null;
            GuardianGreavesKeybind = null;
            MekansmKeybind = null;
            ForceStaffKeybind = null;
            BladeMailKeybind = null;
            ShivasKeybind = null;
            #endregion
        }
    }
}