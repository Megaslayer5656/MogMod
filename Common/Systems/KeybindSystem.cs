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

        public override void Load()
        {
            // Registers a new keybind
            // We localize keybinds by adding a Mods.{ModName}.Keybind.{KeybindName} entry to our localization files. The actual text displayed to English users is in en-US.hjson
            GlimmerCapeKeybind = KeybindLoader.RegisterKeybind(Mod, "GlimmerCape", "G");
            ArmletKeybind = KeybindLoader.RegisterKeybind(Mod, "ArmletOfMordiggian", "T");
        }

        // Please see ExampleMod.cs' Unload() method for a detailed explanation of the unloading process.
        public override void Unload()
        {
            // Not required if your AssemblyLoadContext is unloading properly, but nulling out static fields can help you figure out what's keeping it loaded.
            GlimmerCapeKeybind = null;
            ArmletKeybind = null;
        }
    }
}