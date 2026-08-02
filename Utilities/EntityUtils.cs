using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using System;
using Terraria;

namespace MogMod.Utilities
{
    public static partial class MogModUtils
    {
        /// <summary>
        /// Gets a unit direction towards an arbitrary destination for an entity based on its center. Has <see cref="float.NaN"/> safety in the form of a fallback vector.
        /// </summary>
        /// <param name="entity">The entity to check from.</param>
        /// <param name="destination">The destination to get the direction to.</param>
        /// <param name="fallback">A fallback value to use in the event of an unsafe normalization.</param>
        public static Vector2 SafeDirectionTo(this Entity entity, Vector2 destination, Vector2? fallback = null)
        {
            // Fall back to zero by default. default(Vector2) could be used in the parameter definition, but
            // this is more clear.
            if (!fallback.HasValue)
                fallback = Vector2.Zero;

            return (destination - entity.Center).SafeNormalize(fallback.Value);
        }

        /// <summary>
        /// Sets a player's screenshake value. Automatically checks to ensure it will not override a stronger screenshake.
        /// </summary>
        /// <param name="player">The player to add screenshake to.</param>
        /// <param name="value">The intensity of the screenshake.</param>
        public static void SetScreenshake(this Player player, float value)
        {
            if (player.MogMod().GeneralScreenShakePower < value)
                player.MogMod().GeneralScreenShakePower = value;
        }

        /// <summary>
        /// Adds screenshake to the local player, using the given position and range to determine whether the player is able to see the screenshake.
        /// </summary>
        /// <param name="position">The center of the screenshake, where it is most intense.</param>
        /// <param name="intensity">The maximum intensity of the screenshake.</param>
        /// <param name="range">The distance from which the screenshake's power becomes zero.</param>
        public static void AddScreenshakeAt(Vector2 position, float intensity, float range = 1000)
        {
            float dist = 1;
            dist -= position.Distance(Main.LocalPlayer.Center) / range;

            dist = Math.Max(dist, 0);

            Main.LocalPlayer.GetModPlayer<MogPlayer>().GeneralScreenShakePower += (intensity * dist);
        }

        /// <summary>
        /// Check if Entity is null or Inactive (!active)
        /// </summary>
        /// <param name="entity">Entity to check</param>
        /// <returns>true if entity is null or inactive, otherwise false</returns>
        public static bool IsNullOrInactive(this Entity entity)
        {
            if (entity is null) return true;
            if (!entity.active) return true;

            return false;
        }

        #region Fallback Method for IndexInRange
        /// <summary>
        /// Fallback method for Main.npc.IndexInRange which provide accurate range check (0 <= index < Main.maxNPCs)
        /// </summary>
        /// <param name="index">whoAmI index to check</param>
        /// <returns>true if index is in valid range [0 <= index < Main.maxNPCs]</returns>
        public static bool IndexInRange(this NPC[] _, int index) => (uint)index < Main.maxNPCs;

        /// <summary>
        /// Fallback method for Main.player.IndexInRange which provide accurate range check (0 <= index < Main.maxPlayers)
        /// </summary>
        /// <param name="index">whoAmI index to check</param>
        /// <returns>true if index is in valid range [0 <= index < Main.maxPlayers]</returns>
        public static bool IndexInRange(this Player[] _, int index) => (uint)index < Main.maxPlayers;

        /// <summary>
        /// Fallback method for Main.projectile.IndexInRange which provide accurate range check (0 <= index < Main.maxProjectiles)
        /// </summary>
        /// <param name="index">whoAmI index to check</param>
        /// <returns>true if index is in valid range [0 <= index < Main.maxProjectiles]</returns>
        public static bool IndexInRange(this Projectile[] _, int index) => (uint)index < Main.maxProjectiles;

        /// <summary>
        /// Fallback method for Main.gore.IndexInRange which provide accurate range check (0 <= index < Main.maxGore)
        /// </summary>
        /// <param name="index">whoAmI index to check</param>
        /// <returns>true if index is in valid range [0 <= index < Main.maxGore]</returns>
        public static bool IndexInRange(this Gore[] _, int index) => (uint)index < Main.maxGore;

        /// <summary>
        /// Fallback method for Main.npc.IndexInRange which provide accurate range check (0 <= index < Main.maxDust)
        /// </summary>
        /// <param name="index">whoAmI index to check</param>
        /// <returns>true if index is in valid range [0 <= index < Main.maxDust]</returns>
        public static bool IndexInRange(this Dust[] _, int index) => (uint)index < Main.maxDust;
        #endregion
    }
}
