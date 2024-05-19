using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MogMod.Items.Accessories;

namespace MogMod.Common.Player
{
    public class MogPlayer : ModPlayer
    {
        public bool mewing = false;
        public float mewingguide = 0;
        public enum MewingType
        {
            mewingguide = 0
        }
        public MewingType mewingType = MewingType.mewingguide;
    }
}