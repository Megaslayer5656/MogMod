using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace MogMod.Utilities
{
    public static partial class NPCUtils
    {
        #region Minion Homing
        // gives the real modulo
        public static float Modulo(this float dividend, float divisor)
        {
            return dividend - (float)Math.Floor(dividend / divisor) * divisor;
        }
        // angular distance between two vectors
        public static float AngleBetween(this Vector2 v1, Vector2 v2) => (float)Math.Acos(Vector2.Dot(v1.SafeNormalize(Vector2.Zero), v2.SafeNormalize(Vector2.Zero)));
        public static float AngleBetween(this float angle, float otherAngle) => ((otherAngle - angle) + MathHelper.Pi).Modulo(MathHelper.TwoPi) - MathHelper.Pi;

        /// <summary>
        /// Detects nearby hostile NPCs from a given point
        /// </summary>
        /// <param name="origin">The position where we wish to check for nearby NPCs</param>
        /// <param name="maxDistanceToCheck">Maximum amount of pixels to check around the origin</param>
        /// <param name="ignoreTiles">Whether to ignore tiles when finding a target or not</param>
        /// <param name="bossPriority">Whether bosses should be prioritized in targetting or not</param>
        public static NPC ClosestNPCAt(this Vector2 origin, float maxDistanceToCheck, bool ignoreTiles = true, bool bossPriority = false)
        {
            NPC closestTarget = null;
            float distance = maxDistanceToCheck;
            if (bossPriority)
            {
                bool bossFound = false;
                for (int index = 0; index < Main.npc.Length; index++)
                {
                    // If we've found a valid boss target, ignore ALL targets which aren't bosses.
                    if (bossFound && !(Main.npc[index].boss || Main.npc[index].type == NPCID.WallofFleshEye))
                        continue;

                    if (Main.npc[index].CanBeChasedBy(null, false))
                    {
                        float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);

                        bool canHit = true;
                        if (extraDistance < distance && !ignoreTiles)
                            canHit = Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1);

                        if (Vector2.Distance(origin, Main.npc[index].Center) < distance && canHit)
                        {
                            if (Main.npc[index].boss || Main.npc[index].type == NPCID.WallofFleshEye)
                                bossFound = true;

                            distance = Vector2.Distance(origin, Main.npc[index].Center);
                            closestTarget = Main.npc[index];
                        }
                    }
                }
            }
            else
            {
                for (int index = 0; index < Main.npc.Length; index++)
                {
                    if (Main.npc[index].CanBeChasedBy(null, false))
                    {
                        float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);

                        bool canHit = true;
                        if (extraDistance < distance && !ignoreTiles)
                            canHit = Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1);

                        if (Vector2.Distance(origin, Main.npc[index].Center) < distance && canHit)
                        {
                            distance = Vector2.Distance(origin, Main.npc[index].Center);
                            closestTarget = Main.npc[index];
                        }
                    }
                }
            }
            return closestTarget;
        }

        /// <summary>
        /// Detects the hostile NPC that is closest angle-wise to the rotation vector
        /// </summary>
        /// <param name="origin">The position that will be used to find the rotation vector to NPCs</param>
        /// <param name="checkRotationVector">The rotation vector that the other rotation vectors to NPCs will be compared to</param>
        /// <param name="maxDistanceToCheck">Maximum amount of pixels to check around the origin</param>
        /// <param name="wantedHalfCone">When the angle between the rotation vector and the vector to the NPC is less than or equal to this, NPCs start getting ranked by distance. Set to 0 or less to ignore</param>
        /// <param name="ignoreTiles">Whether or not to ignore tiles when finding a target</param>
        /// <returns>The NPC that best fits the parameters. Null if no NPC is found</returns>
        public static NPC ClosestNPCToAngle(this Vector2 origin, Vector2 checkRotationVector, float maxDistanceToCheck, float wantedHalfCone = 0.125f, bool ignoreTiles = true)
        {
            NPC closestTarget = null;
            float distance = maxDistanceToCheck;
            float angle = MathHelper.Pi;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.CanBeChasedBy(null, false))
                    continue;

                float checkDist = origin.Distance(npc.Center);
                if (checkDist >= distance) // Immediately disqualify anything beyond the distance that must be beaten
                    continue;

                float angleBetween = checkRotationVector.AngleBetween(npc.Center - origin);
                if (angleBetween > angle) // Narrow down to the closest npc to the angle
                    continue;

                if (!ignoreTiles && !Collision.CanHit(origin, 1, 1, npc.Center, 1, 1)) // Tile LoS check if wanted
                    continue;

                if (angle <= wantedHalfCone)
                {
                    angle = wantedHalfCone;
                    distance = checkDist; // We are within the cone. Now npcs are further narrowed down by distance
                    closestTarget = npc;
                }
                else
                {
                    angle = angleBetween;
                    closestTarget = npc;
                }
            }

            return closestTarget;
        }

        /// <summary>
        /// Detects nearby hostile NPCs from a given point with minion support
        /// </summary>
        /// <param name="origin">The position where we wish to check for nearby NPCs</param>
        /// <param name="maxDistanceToCheck">Maximum amount of pixels to check around the origin</param>
        /// <param name="owner">Owner of the minion</param>
        /// <param name="ignoreTiles">Whether to ignore tiles when finding a target or not</param>
        public static NPC MinionHoming(this Vector2 origin, float maxDistanceToCheck, Player owner, bool ignoreTiles = true, bool checksRange = false)
        {
            if (owner is null || !owner.whoAmI.WithinBounds(Main.maxPlayers) || !owner.MinionAttackTargetNPC.WithinBounds(Main.maxNPCs))
                return ClosestNPCAt(origin, maxDistanceToCheck, ignoreTiles);
            NPC npc = Main.npc[owner.MinionAttackTargetNPC];
            bool canHit = true;
            if (!ignoreTiles)
                canHit = Collision.CanHit(origin, 1, 1, npc.Center, 1, 1);
            float extraDistance = (npc.width / 2) + (npc.height / 2);
            bool distCheck = Vector2.Distance(origin, npc.Center) < (maxDistanceToCheck + extraDistance) || !checksRange;
            if (owner.HasMinionAttackTargetNPC && canHit && distCheck)
            {
                return npc;
            }
            return ClosestNPCAt(origin, maxDistanceToCheck, ignoreTiles);
        }
        #endregion
    }
}
