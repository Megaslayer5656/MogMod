using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace MogMod.Common.Config
{
    public class MogClientConfig : ModConfig
    {
        public static MogClientConfig Instance;
        public override ConfigScope Mode => ConfigScope.ClientSide;
        #region Graphics
        [Header("Graphics")]

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool Afterimages { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool GunlanceAmmo { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool TheGravitySpells { get; set; }
        #endregion

        #region UI
        [Header("UI")]

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 1000f)]
        [DefaultValue(UI.GunlanceUI.GunlanceAmmo.GunlanceAmmoPosX)]
        public float GunlanceAmmoPosX { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(-100f, 500f)]
        [DefaultValue(UI.GunlanceUI.GunlanceAmmo.GunlanceAmmoPosY)]
        public float GunlanceAmmoPosY { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 1000f)]
        [DefaultValue(UI.TheGravityUI.TheGravitySpells.TheGravityPosX)]
        public float TheGravityPosX { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(-100f, 500f)]
        [DefaultValue(UI.TheGravityUI.TheGravitySpells.TheGravityPosY)]
        public float TheGravityPosY { get; set; }
        #endregion
    }
}
