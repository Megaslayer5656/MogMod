using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader.Config;

namespace MogMod.Common.Config
{
    public class MogClientConfig : ModConfig
    {
        public static MogClientConfig Instance;
        public override ConfigScope Mode => ConfigScope.ClientSide;
        #region Graphics Changes
        [Header("Graphics")]

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool Afterimages { get; set; }
        #endregion
    }
}
