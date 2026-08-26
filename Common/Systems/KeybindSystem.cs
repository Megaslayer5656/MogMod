using Terraria.ModLoader;

namespace MogMod.Common.Systems
{
    public class KeybindSystem : ModSystem
    {
        #region Keybind Setup
        public static ModKeybind GlimmerCapeKeybind { get; private set; }
        public static ModKeybind ArmletKeybind { get; private set; }
        public static ModKeybind SatanicKeybind { get; private set; }
        public static ModKeybind BootsKeybind { get; private set; }
        public static ModKeybind RefresherOrbKeybind { get; private set; }
        public static ModKeybind WandKeybind { get; private set; }
        public static ModKeybind MekansmKeybind { get; private set; }
        public static ModKeybind BladeMailKeybind { get; private set; }
        public static ModKeybind ShivasKeybind {  get; private set; }
        public static ModKeybind DragonInstallKeybind {  get; private set; }
        public static ModKeybind TheGravityKeybind { get; private set; }
        public static ModKeybind NulledKeybind { get; private set; }
        public static ModKeybind RigKeybind { get; private set; }
        #endregion
        public override void Load()
        {
            // Registers a new keybind
            // We localize keybinds by adding a Mods.{ModName}.Keybind.{KeybindName} entry to our localization files. The actual text displayed to English users is in en-US.hjson

            #region Healing/Mana
            RefresherOrbKeybind = KeybindLoader.RegisterKeybind(Mod, "ActivateRefresherOrb", "V");
            BootsKeybind = KeybindLoader.RegisterKeybind(Mod, "ActivateBootEffects", "C");
            WandKeybind = KeybindLoader.RegisterKeybind(Mod, "ActivateMagicWand", "Z");
            MekansmKeybind = KeybindLoader.RegisterKeybind(Mod, "ActivateMekansm", "C");
            #endregion

            #region Defensive/Mobility
            GlimmerCapeKeybind = KeybindLoader.RegisterKeybind(Mod, "GlimmerCape", "X");
            SatanicKeybind = KeybindLoader.RegisterKeybind(Mod, "ActivateSatanic", "X");
            DragonInstallKeybind = KeybindLoader.RegisterKeybind(Mod, "DragonInstall", "F");
            #endregion

            #region Attack
            ShivasKeybind = KeybindLoader.RegisterKeybind(Mod, "ActivateShiva'sGuard", "C");
            BladeMailKeybind = KeybindLoader.RegisterKeybind(Mod, "ActivateBladeMail", "X");
            ArmletKeybind = KeybindLoader.RegisterKeybind(Mod, "ToggleArmletOfMordiggian", "T");
            NulledKeybind = KeybindLoader.RegisterKeybind(Mod, "ToggleNulled", "T");
            TheGravityKeybind = KeybindLoader.RegisterKeybind(Mod, "TheGravityCardSwitch", "Mouse4");
            #endregion

            #region Other
            RigKeybind = KeybindLoader.RegisterKeybind(Mod, "ActivateRigEffect", "G");
            #endregion
        }

        public override void Unload()
        {
            #region Keybind Reset
            // Not required if your AssemblyLoadContext is unloading properly, but nulling out static fields can help you figure out what's keeping it loaded.
            GlimmerCapeKeybind = null;
            ArmletKeybind = null;
            SatanicKeybind = null;
            BootsKeybind = null;
            RefresherOrbKeybind = null;
            WandKeybind = null;
            MekansmKeybind = null;
            BladeMailKeybind = null;
            ShivasKeybind = null;
            DragonInstallKeybind = null;
            TheGravityKeybind = null;
            NulledKeybind = null;
            RigKeybind = null;
            #endregion
        }
    }
}
