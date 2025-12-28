using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

namespace MogMod.Utilities
{
    public static partial class MiscUtils
    {
        public static void AddWithCondition<T>(this List<T> list, T type, bool condition)
        {
            if (condition)
                list.Add(type);
        }
    }
}
