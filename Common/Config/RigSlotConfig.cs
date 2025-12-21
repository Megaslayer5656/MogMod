using MogMod.Common.Systems;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace MogMod.Common.Config
{
    internal class RigSlotConfig : ModConfig
    {
        public enum Location
        {
            Default,
            Custom
        }

        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("SlotLocation")]
        [DefaultValue(Location.Default)]
        [DrawTicks]
        public Location SlotLocation;

        [DefaultValue(true)]
        public bool ShowCustomLocationPanel;

        [DefaultValue(false)]
        public bool ResetCustomSlotLocation;

        public override void OnChanged()
        {
            if (SlotLocation == Location.Default)
            {
                ShowCustomLocationPanel = false;
            }

            if (RigSlotSystem.UI != null)
            {
                RigSlotSystem.UI.Panel.Visible = ShowCustomLocationPanel;
                RigSlotSystem.UI.Panel.CanDrag = ShowCustomLocationPanel;

                if (ResetCustomSlotLocation)
                {
                    RigSlotSystem.UI.ResetPosition();
                    ResetCustomSlotLocation = false;
                }
            }
        }
    }
}