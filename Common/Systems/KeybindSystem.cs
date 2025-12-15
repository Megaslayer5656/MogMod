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
        public static ModKeybind MagicStickKeybind { get; private set; }
        public static ModKeybind HelmOfDominatorKeybind { get; private set; }
        

        public override void Load()
        {
            // Registers a new keybind
            // We localize keybinds by adding a Mods.{ModName}.Keybind.{KeybindName} entry to our localization files. The actual text displayed to English users is in en-US.hjson
            GlimmerCapeKeybind = KeybindLoader.RegisterKeybind(Mod, "GlimmerCape", "G");
            ArmletKeybind = KeybindLoader.RegisterKeybind(Mod, "ArmletOfMordiggian", "T");
            SatanicKeybind = KeybindLoader.RegisterKeybind(Mod, "Satanic", "G");
            ArcaneBootsKeybind = KeybindLoader.RegisterKeybind(Mod, "ArcaneBoots", "G");
            RefresherOrbKeybind = KeybindLoader.RegisterKeybind(Mod, "RefresherOrb", "V");
            WandKeybind = KeybindLoader.RegisterKeybind(Mod, "MagicWand", "C");
            MagicStickKeybind = KeybindLoader.RegisterKeybind(Mod, "MagicStick", "C");
            HelmOfDominatorKeybind = KeybindLoader.RegisterKeybind(Mod, "HelmOfTheDominator", "V");
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
            MagicStickKeybind = null;
            HelmOfDominatorKeybind = null;
        }
    }
}