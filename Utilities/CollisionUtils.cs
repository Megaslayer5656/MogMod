using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace MogMod.Utilities
{
    public static partial class MogModUtils
    {
        /// <summary>
        /// Performs collision based a rotating hitbox for an entity by treating the hitbox as a line. By default uses the velocity of the entity as a direction. This can be overriden.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="targetTopLeft">The top left coordinates of the target to check.</param>
        /// <param name="targetHitboxDimensions">The hitbox size of the target to check.</param>
        /// <param name="directionOverride">An optional direction override</param>
        public static bool RotatingHitboxCollision(this Entity entity, Vector2 targetTopLeft, Vector2 targetHitboxDimensions, Vector2? directionOverride = null, float scale = 1f)
        {
            Vector2 lineDirection = directionOverride ?? entity.velocity;

            // Ensure that the line direction is a unit vector.
            lineDirection = lineDirection.SafeNormalize(Vector2.UnitY);
            Vector2 start = entity.Center - lineDirection * entity.height * 0.5f * scale;
            Vector2 end = entity.Center + lineDirection * entity.height * 0.5f * scale;

            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetTopLeft, targetHitboxDimensions, start, end, entity.width * scale, ref _);
        }
        /// <summary>
        /// Shortcut used to make projectiles have rotating hitbox collision.
        /// </summary>
        /// <param name="projectile">The projectile.</param>
        /// <param name="targetHitbox">The rectangle for the target hitbox.</param>
        public static bool RotatingHitboxCollision(this Projectile proj, Rectangle targetHitbox) => RotatingHitboxCollision(proj, targetHitbox.TopLeft(), targetHitbox.Size(), (proj.rotation + MathHelper.PiOver2).ToRotationVector2(), proj.scale);

        /// <summary>
        /// Determines if a tile is solid based on whether it's active and not actuated or if the tile is solid. This will not count platforms and other non-solid ground tiles
        /// </summary>
        /// <param name="tile">The tile to check.</param>
        public static bool IsTileSolid(this Tile tile) => tile != null && tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType];

        /// <summary>
        /// Determines the distance required before a ray in a given direction from a given starting position hits solid tiles, taking slopes into account.
        /// </summary>
        /// <param name="start">The point to check from.</param>
        /// <param name="rotation">The direction in which tiles are checked.</param>
        /// <param name="length">How far in the direction that will be checked.</param>
        /// <param name="step">How many units moved forward each loop. Greater = less precise.</param>
        /// <returns>The length until the first collision detected. Returns input length if no collision occurs.</returns>
        public static float PreciseDistanceToTileCollisionHit(Vector2 start, float rotation, float length, float step = 1)
        {
            Vector2 unitVect = rotation.ToRotationVector2();
            Vector2 end = unitVect * length;

            if (length < 1f)
            {
                Point endWorldPos = end.ToTileCoordinates();
                return MiscUtils.ParanoidTileRetrieval(endWorldPos.X, endWorldPos.Y).IsTileSolid() ? 0 : length;
            }

            Vector2 currentPos = start;
            Point lastAirPos = new(-1, -1);
            for (float i = 0; i < length; i += step)
            {
                currentPos += unitVect * step;

                Point tilePos = currentPos.ToTileCoordinates();

                if (tilePos == lastAirPos)
                    continue;

                if (!WorldGen.InWorld(tilePos.X, tilePos.Y))
                    continue;

                Tile tile = Main.tile[tilePos.X, tilePos.Y];
                if (!tile.IsTileSolid())
                {
                    lastAirPos = tilePos;
                    continue;
                }

                if (tile.Slope == SlopeType.Solid && !tile.IsHalfBlock)
                    return (currentPos - start).Length();

                Vector2 tileWorldPos = new(tilePos.X * 16, tilePos.Y * 16);
                Vector2 currentPosInTile = currentPos - tileWorldPos;
                if (tile.IsHalfBlock)
                {
                    if (currentPosInTile.Y >= 8f)
                        return (currentPos - start).Length();
                }
                else if (tile.Slope == SlopeType.SlopeDownLeft)
                {
                    if (currentPosInTile.X <= currentPosInTile.Y)
                        return (currentPos - start).Length();
                }
                else if (tile.Slope == SlopeType.SlopeDownRight)
                {
                    if ((16 - currentPosInTile.X) <= currentPosInTile.Y)
                        return (currentPos - start).Length();
                }
                else if (tile.Slope == SlopeType.SlopeUpLeft)
                {
                    if (currentPosInTile.X <= (16 - currentPosInTile.Y))
                        return (currentPos - start).Length();
                }
                else if (tile.Slope == SlopeType.SlopeUpRight)
                {
                    if (currentPosInTile.X >= currentPosInTile.Y)
                        return (currentPos - start).Length();
                }
            }

            return length;
        }
    }
}
