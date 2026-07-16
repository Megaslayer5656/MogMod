using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Utilities
{
    public static partial class MiscUtils
    {
        // all from calamity mod thank you 
        /// <param name="key">The language key. This will have "Mods.MogMod." appended behind it.</param>
        /// <returns>
        /// A <see cref="LocalizedText"/> instance found using the provided key with "Mods.MogMod." appended behind it. 
        /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
        /// </returns>
        public static LocalizedText GetText(string key)
        {
            return Language.GetOrRegister("Mods.MogMod." + key);
        }
        /// <param name="key">The language key. This will have "Mods.MogMod." appended behind it.</param>
        /// <returns>
        /// A <see cref="string"/> instance found using the provided key with "Mods.MogMod." appended behind it.
        /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
        /// </returns>
        public static string GetTextValue(string key)
        {
            return Language.GetTextValue("Mods.MogMod." + key);
        }
        /// <param name="itemID">The item's ID.</param>
        /// <returns>
        /// A <see cref="LocalizedText"/> instance for an item's name. 
        /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
        /// </returns>
        public static LocalizedText GetItemName(int itemID)
        {
            if (itemID < ItemID.Count)
            {
                return Language.GetText("ItemName." + ItemID.Search.GetName(itemID));
            }
            return GetTextFromModItem(itemID, "DisplayName");
        }

        /// <returns>
        /// A <see cref="LocalizedText"/> instance which will have the item's translated name.
        /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
        /// </returns>
        public static LocalizedText GetItemName<T>() where T : ModItem => GetTextFromModItem(ModContent.ItemType<T>(), "DisplayName");
        /// <param name="itemID">The item's ID.</param>
        /// <param name="suffix">The desired suffix.</param>
        /// <returns>
        /// A <see cref="LocalizedText"/> instance for the given item and suffix
        /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
        /// </returns>
        public static LocalizedText GetTextFromModItem(int itemID, string suffix)
        {
            var modItem = ItemLoader.GetItem(itemID);
            return modItem.GetLocalization(suffix);
        }
        /// <param name="suffix">The desired suffix.</param>
        /// <returns>
        /// A <see cref="LocalizedText"/> instance for the given item and suffix
        /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
        /// </returns>
        public static LocalizedText GetTextFromModItem<T>(string suffix) where T : ModItem => GetTextFromModItem(ModContent.ItemType<T>(), suffix);

        /// <param name="itemID">The item's ID.</param>
        /// <param name="suffix">The desired suffix.</param>
        /// <returns>
        /// A <see cref="string"/> instance for the given item and suffix
        /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
        /// </returns>
        public static string GetTextValueFromModItem(int itemID, string suffix) => GetTextFromModItem(itemID, suffix).ToString();

        /// <param name="suffix">The desired suffix.</param>
        /// <returns>
        /// A <see cref="string"/> instance for the given item and suffix
        /// <para>NOTE: Modded translations are not loaded until after PostSetupContent.</para>Caching the result is suggested.
        /// </returns>
        public static string GetTextValueFromModItem<T>(string suffix) where T : ModItem => GetTextFromModItem(ModContent.ItemType<T>(), suffix).ToString();
        /// <summary>
        /// Broadcast a LocalizedText. This only should be run on Singleplayer or Server.
        /// Multiplayer Clients Do NOT ask Server to Broadcast nor the print message locally.
        /// </summary>
        /// <param name="key">LocalizedText key</param>
        /// <param name="textColor">Text Color to use</param>
        public static void BroadcastLocalizedText(string key, Color? textColor = null)
        {
            // An attempt to bypass the need for a separate method and runtime/compile-time parameter
            // constraints by using nulls for defaults.
            if (!textColor.HasValue)
                textColor = Color.White;

            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(key), textColor.Value);
            else if (Main.dedServ)
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(key), textColor.Value);
        }

        #region Tooltip Format Helper
        public static string EmbedItemIcon(this int itemID) => $"[i:{itemID}] " + GetItemName(itemID);

        public static string FramesToSeconds(this int frame) => Round(frame / 60f, "N2");
        public static string FramesToMinutes(this int frame) => Round(frame / 60f / 60f, "N2");
        public static string ToMph(this float velocity) => Round(velocity * 216000f / 42240f, "N0");
        public static string ToMphps(this float velocity) => Round(velocity * 60f * 216000f / 42240f, "N2");
        public static string ToTiles(this float pixel) => Round(pixel / 16f);
        public static string ToReversedPercent(this float percent) => Round((1 - percent) * 100);
        public static string ToRegenPerSecond(this float partialRegen) => Round(partialRegen * 0.5f, "N2");
        public static string ToRegenPerSecond(this int regen, float partialRegen = 0f) => Round((regen + partialRegen) * 0.5f, "N2");
        public static string ToJumpSpeedPercent(this float boost) => Round(boost * 20f, "N2");
        public static string ToStealth(this float stealth) => Round(stealth * 100f, "N0");

        public static string GetChanceFromDenominator(this int denominator) => ToPercent(1 / (float)denominator);

        public static string ToPercent(this int percent) => (percent * 100).ToString();
        public static string ToPercent(this float percent, string precision = "N1") => Round(percent * 100f, precision);
        public static string ToPercent(this double percent, string precision = "N1") => Round(percent * 100D, precision);
        // Double-rounded for proper digit cutoffs
        public static string Round(this float number, string precision = "N4") => float.Parse((number).ToString(precision)).ToString();
        public static string Round(this double number, string precision = "N4") => float.Parse((number).ToString(precision)).ToString();
        #endregion
        public static void AddWithCondition<T>(this List<T> list, T type, bool condition)
        {
            if (condition)
                list.Add(type);
        }
        public static bool WithinBounds(this int index, int cap) => index >= 0 && index < cap;
        public delegate float EasingFunction(float amount, int degree);
        public static float LinearEasing(float amount, int degree) => amount;
        //Sines
        public static float SineInEasing(float amount, int degree) => 1f - (float)Math.Cos(amount * MathHelper.Pi / 2f);
        public static float SineOutEasing(float amount, int degree) => (float)Math.Sin(amount * MathHelper.Pi / 2f);
        public static float SineInOutEasing(float amount, int degree) => -((float)Math.Cos(amount * MathHelper.Pi) - 1) / 2f;
        public static float SineBumpEasing(float amount, int degree) => (float)Math.Sin(amount * MathHelper.Pi);
        //Polynomials
        public static float PolyInEasing(float amount, int degree) => (float)Math.Pow(amount, degree);
        public static float PolyOutEasing(float amount, int degree) => 1f - (float)Math.Pow(1f - amount, degree);
        public static float PolyInOutEasing(float amount, int degree) => amount < 0.5f ? (float)Math.Pow(2, degree - 1) * (float)Math.Pow(amount, degree) : 1f - (float)Math.Pow(-2 * amount + 2, degree) / 2f;
        //Exponential
        public static float ExpInEasing(float amount, int degree) => amount == 0f ? 0f : (float)Math.Pow(2, 10f * amount - 10f);
        public static float ExpOutEasing(float amount, int degree) => amount == 1f ? 1f : 1f - (float)Math.Pow(2, -10f * amount);
        public static float ExpInOutEasing(float amount, int degree) => amount == 0f ? 0f : amount == 1f ? 1f : amount < 0.5f ? (float)Math.Pow(2, 20f * amount - 10f) / 2f : (2f - (float)Math.Pow(2, -20f * amount - 10f)) / 2f;
        //circular
        public static float CircInEasing(float amount, int degree) => (1f - (float)Math.Sqrt(1 - Math.Pow(amount, 2f)));
        public static float CircOutEasing(float amount, int degree) => (float)Math.Sqrt(1 - Math.Pow(amount - 1f, 2f));
        public static float CircInOutEasing(float amount, int degree) => amount < 0.5 ? (1f - (float)Math.Sqrt(1 - Math.Pow(2 * amount, 2f))) / 2f : ((float)Math.Sqrt(1 - Math.Pow(-2f * amount - 2f, 2f)) + 1f) / 2f;
        public enum EasingType
        {
            Linear,
            SineIn, SineOut, SineInOut, SineBump,
            PolyIn, PolyOut, PolyInOut,
            ExpIn, ExpOut, ExpInOut,
            CircIn, CircOut, CircInOut
        }
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
        /// <summary>
        /// Determines if a tile is solid ground based on whether it's active and not actuated or if the tile is solid in any way, including just the top.
        /// </summary>
        /// <param name="tile">The tile to check.</param>
        public static bool IsTileSolidGround(this Tile tile) => tile != null && tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]);
        public static Tile ParanoidTileRetrieval(int x, int y)
        {
            if (!WorldGen.InWorld(x, y))
                return new Tile();

            return Main.tile[x, y];
        }
        /// <summary>
        /// Gets the sign of the number, but without the zero case. If 0 is inputted into this method, 1 is returned/
        /// </summary>
        /// <param name="x">The input value.</param>
        public static int DirectionalSign(this float x) => (x > 0f).ToDirectionInt();
        private static readonly EasingFunction[] EasingTypeToFunction = new EasingFunction[] { LinearEasing, SineInEasing, SineOutEasing, SineInOutEasing, SineBumpEasing, PolyInEasing, PolyOutEasing, PolyInOutEasing, ExpInEasing, ExpOutEasing, ExpInOutEasing, CircInEasing, CircOutEasing, CircInOutEasing };
        /// <summary>
        /// This represents a part of a piecewise function.
        /// </summary>
        public struct CurveSegment
        {
            /// <summary>
            /// This is the type of easing used in the segment
            /// </summary>
            public EasingFunction easing;
            /// <summary>
            /// This indicates when the segment starts on the animation
            /// </summary>
            public float startingX;
            /// <summary>
            /// This indicates what the starting height of the segment is
            /// </summary>
            public float startingHeight;
            /// <summary>
            /// This represents the elevation shift that will happen during the segment. Set this to 0 to turn the segment into a flat line.
            /// Usually this elevation shift is fully applied at the end of a segment, but the sinebump easing type makes it be reached at the apex of its curve.
            /// </summary>
            public float elevationShift;
            /// <summary>
            /// This is the degree of the polynomial, if the easing mode chosen is a polynomial one
            /// </summary>
            public int degree;

            /// <summary>
            /// The height of the segment after the elevation shift is taken into account.
            /// </summary>
            public float EndingHeight => startingHeight + elevationShift;

            /// <summary>
            /// Legacy constructor
            /// </summary>
            public CurveSegment(EasingType MODE, float startX, float startHeight, float elevationShift, int degree = 1) :
                this(EasingTypeToFunction[(int)MODE], startX, startHeight, elevationShift, degree)
            { }

            public CurveSegment(EasingFunction MODE, float startX, float startHeight, float elevationShift, int degree = 1)
            {
                easing = MODE;
                startingX = startX;
                startingHeight = startHeight;
                this.elevationShift = elevationShift;
                this.degree = degree;
            }
        }
        /// <summary>
        /// This gives you the height of a custom piecewise function for any given X value, so you may create your own complex animation curves easily.
        /// The X value is automatically clamped between 0 and 1, but the height of the function may go beyond the 0 - 1 range
        /// </summary>
        /// <param name="progress">How far along the curve you are. Automatically clamped between 0 and 1</param>
        /// <param name="segments">An array of curve segments making up the full animation curve</param>
        /// <returns></returns>
        public static float PiecewiseAnimation(float progress, params CurveSegment[] segments)
        {
            if (segments.Length == 0)
                return 0f;

            if (segments[0].startingX != 0) //If for whatever reason you try to not play by the rules, get fucked
                segments[0].startingX = 0;

            progress = MathHelper.Clamp(progress, 0f, 1f); //Clamp the progress
            float ratio = 0f;

            for (int i = 0; i <= segments.Length - 1; i++)
            {
                CurveSegment segment = segments[i];
                float startPoint = segment.startingX;
                float endPoint = 1f;

                if (progress < segment.startingX) //Too early. This should never get reached, since by the time you'd have gotten there you'd have found the appropriate segment and broken out of the for loop
                    continue;

                if (i < segments.Length - 1)
                {
                    if (segments[i + 1].startingX <= progress) //Too late
                        continue;
                    endPoint = segments[i + 1].startingX;
                }

                float segmentLength = endPoint - startPoint;
                float segmentProgress = (progress - segment.startingX) / segmentLength; //How far along the specific segment
                ratio = segment.startingHeight;

                //Failsafe because somehow it can fail? what
                if (segment.easing != null)
                    ratio += segment.easing(segmentProgress, segment.degree) * segment.elevationShift;

                else
                    ratio += LinearEasing(segmentProgress, segment.degree) * segment.elevationShift;

                break;
            }
            return ratio;
        }
        /// <summary>
        /// Used to limit the cursor up to a 1080p monitor. A similar method is used for items such as Zenith in vanilla.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>The current position of the player's mouse, clamped to a 1920x1080 screen.</returns>
        public static Vector2 ClampedMouseWorld(this Player player)
        {
            Vector2 mouseWorld = player.MogMod().mouseWorld;

            // Clamp each axis
            mouseWorld.X = mouseWorld.X >= player.MountedCenter.X ? MathF.Min(mouseWorld.X, player.MountedCenter.X + 960f) : MathF.Max(mouseWorld.X, player.MountedCenter.X - 960f);
            mouseWorld.Y = mouseWorld.Y >= player.MountedCenter.Y ? MathF.Min(mouseWorld.Y, player.MountedCenter.Y + 540f) : MathF.Max(mouseWorld.Y, player.MountedCenter.Y - 540f);
            return mouseWorld;
        }
    }
}
