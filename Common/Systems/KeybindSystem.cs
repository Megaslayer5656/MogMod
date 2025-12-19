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


        public override void Load()
        {
            // Registers a new keybind
            // We localize keybinds by adding a Mods.{ModName}.Keybind.{KeybindName} entry to our localization files. The actual text displayed to English users is in en-US.hjson
            ArmletKeybind = KeybindLoader.RegisterKeybind(Mod, "ArmletOfMordiggian", "T");
            GlimmerCapeKeybind = KeybindLoader.RegisterKeybind(Mod, "GlimmerCape", "G");
            SatanicKeybind = KeybindLoader.RegisterKeybind(Mod, "Satanic", "C");
            RefresherOrbKeybind = KeybindLoader.RegisterKeybind(Mod, "RefresherOrb", "X");
            WandKeybind = KeybindLoader.RegisterKeybind(Mod, "MagicWand", "C");
            HelmOfDominatorKeybind = KeybindLoader.RegisterKeybind(Mod, "HelmOfTheDominator", "V");
            MekansmKeybind = KeybindLoader.RegisterKeybind(Mod, "Mekansm", "X");
            ArcaneBootsKeybind = KeybindLoader.RegisterKeybind(Mod, "ArcaneBoots", "Z");
            GuardianGreavesKeybind = KeybindLoader.RegisterKeybind(Mod, "GuardianGreaves", "Z");
            ForceStaffKeybind = KeybindLoader.RegisterKeybind(Mod, "ForceStaff", "V");
            BladeMailKeybind = KeybindLoader.RegisterKeybind(Mod, "BladeMail", "C");
            ShivasKeybind = KeybindLoader.RegisterKeybind(Mod, "Shiva'sGuard", "C");
        }

        public override void Unload()
        {
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
        }
    }
}