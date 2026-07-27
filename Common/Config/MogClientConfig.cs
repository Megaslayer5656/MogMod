using MogMod.Utilities;
using System.ComponentModel;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace MogMod.Common.Config
{
    [BackgroundColor(32, 40, 49, 216)]
    public class MogClientConfig : ModConfig
    {
        public static MogClientConfig Instance;
        public override ConfigScope Mode => ConfigScope.ClientSide;
        #region Graphics
        [Header("Graphics")]

        [BackgroundColor(171, 54, 192, 192)]
        [DefaultValue(true)]
        public bool Afterimages { get; set; }

        [BackgroundColor(171, 54, 192, 192)]
        [DefaultValue(true)]
        public bool GunlanceAmmo { get; set; }

        [BackgroundColor(171, 54, 192, 192)]
        [DefaultValue(true)]
        public bool TheGravitySpells { get; set; }
        #endregion

        #region UI
        [Header("UI")]

        [BackgroundColor(54, 192, 162, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 1000f)]
        [DefaultValue(UI.GunlanceUI.GunlanceAmmo.GunlanceAmmoPosX)]
        public float GunlanceAmmoPosX { get; set; }

        [BackgroundColor(54, 192, 162, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(-100f, 500f)]
        [DefaultValue(UI.GunlanceUI.GunlanceAmmo.GunlanceAmmoPosY)]
        public float GunlanceAmmoPosY { get; set; }

        [BackgroundColor(54, 192, 162, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 1000f)]
        [DefaultValue(UI.TheGravityUI.TheGravitySpells.TheGravityPosX)]
        public float TheGravityPosX { get; set; }

        [BackgroundColor(54, 192, 162, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(-100f, 500f)]
        [DefaultValue(UI.TheGravityUI.TheGravitySpells.TheGravityPosY)]
        public float TheGravityPosY { get; set; }
        #endregion

        #region Weapon Visuals
        [Header("WeaponVisuals")]
        [BackgroundColor(192, 137, 54, 192)]
        [DefaultValue(true)]
        public bool GunRecoil { get; set; }

        [BackgroundColor(192, 137, 54, 192)]
        [DefaultValue(true)]
        public bool AmmoEjection { get; set; }
        #endregion

    }
    [BackgroundColor(33, 49, 32, 216)]
    public class MogServerConfig : ModConfig
    {
        public static MogServerConfig Instance;
        public override ConfigScope Mode => ConfigScope.ServerSide;
        // only server host can change server config
        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message)
        {
            if (whoAmI == 0)
            {
                return true;
            }
            if (whoAmI != 0)
            {
                message = MiscUtils.GetText("Configs.MogServerConfig.Denied").ToNetworkText();
                return false;
            }
            return false;
        }
        #region Gameplay
        [Header("Gameplay")]
        [BackgroundColor(192, 54, 82, 192)]
        [DefaultValue(true)]
        public bool EliteEnemySpawning { get; set; }
        #endregion
    }
}