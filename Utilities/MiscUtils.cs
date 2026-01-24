using System.Collections.Generic;
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
        public static bool WithinBounds(this int index, int cap) => index >= 0 && index < cap;
        public static Vector2 SafeDirectionTo(this Entity entity, Vector2 destination, Vector2? fallback = null)
        {
            // Fall back to zero by default. default(Vector2) could be used in the parameter definition, but
            // this is more clear.
            if (!fallback.HasValue)
                fallback = Vector2.Zero;

            return (destination - entity.Center).SafeNormalize(fallback.Value);
        }
        public static Tile TileRetrieval(int x, int y)
        {
            if (!WorldGen.InWorld(x, y))
                return new Tile();

            return Main.tile[x, y];
        }
    }
}
