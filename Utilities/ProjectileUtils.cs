using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Config;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories.Rigs;
using MogMod.NPCs.Global;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Utilities
{
    public static partial class MogModUtils
    {
        #region Setup
        public static T ModProjectile<T>(this Projectile projectile) where T : ModProjectile
        {
            return projectile.ModProjectile as T;
        }
        public static bool AnyProjectiles(int projectileID)
        {
            // Efficiently loop through all projectiles, using a specially designed continue continue that attempts to minimize the amount of OR
            // checks per iteration.
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.type != projectileID)
                    continue;

                return true;
            }

            return false;
        }
        public static Vector2 RandomVelocity(float directionMult, float speedLowerLimit, float speedCap, float speedMult = 0.1f)
        {
            Vector2 velocity = new Vector2(Main.rand.NextFloat(-directionMult, directionMult), Main.rand.NextFloat(-directionMult, directionMult));
            //Rerolling to avoid dividing by zero
            while (velocity.X == 0f && velocity.Y == 0f)
            {
                velocity = new Vector2(Main.rand.NextFloat(-directionMult, directionMult), Main.rand.NextFloat(-directionMult, directionMult));
            }
            velocity.Normalize();
            velocity *= Main.rand.NextFloat(speedLowerLimit, speedCap) * speedMult;
            return velocity;
        }
        #endregion

        #region Projectile Spawning
        // summons projectiles from the sky
        public static Projectile ProjectileRain(IEntitySource source, Vector2 targetPos, float xLimit, float xVariance, float yLimitLower, float yLimitUpper, float projSpeed, int projType, int damage, float knockback, int owner, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f)
        {
            float x = targetPos.X + Main.rand.NextFloat(-xLimit, xLimit);
            float y = targetPos.Y - Main.rand.NextFloat(yLimitLower, yLimitUpper);
            Vector2 spawnPosition = new(x, y);
            Vector2 velocity = targetPos - spawnPosition;
            velocity.X += Main.rand.NextFloat(-xVariance, xVariance);
            float speed = projSpeed;
            float targetDist = velocity.Length();
            targetDist = speed / targetDist;
            velocity.X *= targetDist;
            velocity.Y *= targetDist;
            return Projectile.NewProjectileDirect(source, spawnPosition, velocity, projType, damage, knockback, owner, ai0, ai1, ai2);
        }

        // summons projectiles from the sides of the target
        public static Projectile ProjectileBarrage(IEntitySource source, Vector2 originVec, Vector2 targetPos, bool fromRight, float xOffsetMin, float xOffsetMax, float yOffsetMin, float yOffsetMax, float projSpeed, int projType, int damage, float knockback, int owner, bool clamped = false, float inaccuracyOffset = 5f, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f)
        {
            float xPos = originVec.X + Main.rand.NextFloat(xOffsetMin, xOffsetMax) * fromRight.ToDirectionInt();
            float yPos = originVec.Y + Main.rand.NextFloat(yOffsetMin, yOffsetMax) * Main.rand.NextBool().ToDirectionInt();
            Vector2 spawnPosition = new(xPos, yPos);
            Vector2 velocity = targetPos - spawnPosition;
            velocity.X += Main.rand.NextFloat(-inaccuracyOffset, inaccuracyOffset);
            velocity.Y += Main.rand.NextFloat(-inaccuracyOffset, inaccuracyOffset);
            velocity.Normalize();
            velocity *= projSpeed * (clamped ? 150f : 1f);
            //This clamp means the spawned projectiles only go at diagnals and are not accurate
            if (clamped)
            {
                velocity.X = MathHelper.Clamp(velocity.X, -15f, 15f);
                velocity.Y = MathHelper.Clamp(velocity.Y, -15f, 15f);
            }
            return Projectile.NewProjectileDirect(source, spawnPosition, velocity, projType, damage, knockback, owner, ai0, ai1, ai2);
        }

        // homing code from calamity
        public static void HomeInOnNPC(Projectile projectile, bool ignoreTiles, float distanceRequired, float homingVelocity, float inertia, bool extraUpdates = true)
        {
            if (!projectile.friendly)
                return;

            // Set amount of extra updates.
            if (projectile.MogMod().defExtraUpdates == -1 && extraUpdates)
                projectile.MogMod().defExtraUpdates = projectile.extraUpdates;

            Vector2 destination = projectile.Center;
            float maxDistance = distanceRequired;
            bool locatedTarget = false;

            // Find the closest target.
            float npcDistCompare = 25000f; // Initializing the value to a large number so the first entry is basically guaranteed to replace it.
            int index = -1;
            // ignore red line under Main.ActiveNPCs
            foreach (NPC n in Main.ActiveNPCs)
            {
                float extraDistance = (n.width / 2) + (n.height / 2);
                if (!n.CanBeChasedBy(projectile, false) || !projectile.WithinRange(n.Center, maxDistance + extraDistance))
                    continue;

                float currentNPCDist = Vector2.Distance(n.Center, projectile.Center);
                if ((currentNPCDist < npcDistCompare) && (ignoreTiles || Collision.CanHit(projectile.Center, 1, 1, n.Center, 1, 1)))
                {
                    npcDistCompare = currentNPCDist;
                    index = n.whoAmI;
                }
            }
            // If the index was never changed, don't do anything. Otherwise, tell the projectile where to home.
            if (index != -1)
            {
                destination = Main.npc[index].Center;
                locatedTarget = true;
            }

            if (locatedTarget)
            {
                // Increase amount of extra updates to greatly increase homing velocity.
                if (extraUpdates) projectile.extraUpdates = projectile.MogMod().defExtraUpdates + 1;

                // Home in on the target.
                Vector2 homeDirection = (destination - projectile.Center).SafeNormalize(Vector2.UnitY);
                projectile.velocity = (projectile.velocity * inertia + homeDirection * homingVelocity) / (inertia + 1f);
            }
            else
            {
                // Set amount of extra updates to default amount.
                if (extraUpdates) projectile.extraUpdates = projectile.MogMod().defExtraUpdates;
            }
        }
        public static void HomeInOnMarkedNPC(Projectile projectile, bool ignoreTiles, float distanceRequired, float homingVelocity, float inertia)
        {
            if (!projectile.friendly)
                return;

            // Set amount of extra updates.
            if (projectile.MogMod().defExtraUpdates == -1)
                projectile.MogMod().defExtraUpdates = projectile.extraUpdates;

            Vector2 destination = projectile.Center;
            float maxDistance = distanceRequired;
            bool locatedTarget = false;

            // Find the closest target.
            float npcDistCompare = 25000f; // Initializing the value to a large number so the first entry is basically guaranteed to replace it.
            int index = -1;
            // ignore red line under Main.ActiveNPCs
            foreach (NPC n in Main.ActiveNPCs)
            {
                float extraDistance = (n.width / 2) + (n.height / 2);
                if (!n.CanBeChasedBy(projectile, false) || !projectile.WithinRange(n.Center, maxDistance + extraDistance))
                    continue;

                if (!n.TryGetGlobalNPC<MogModGlobalNPC>(out var globalNPC))
                    continue;

                if (!globalNPC.markedByMarker)
                    continue;

                float currentNPCDist = Vector2.Distance(n.Center, projectile.Center);
                if ((currentNPCDist < npcDistCompare) && (ignoreTiles || Collision.CanHit(projectile.Center, 1, 1, n.Center, 1, 1)))
                {
                    npcDistCompare = currentNPCDist;
                    index = n.whoAmI;
                }
            }
            // If the index was never changed, don't do anything. Otherwise, tell the projectile where to home.
            if (index != -1)
            {
                destination = Main.npc[index].Center;
                locatedTarget = true;
            }

            if (locatedTarget)
            {
                // Increase amount of extra updates to greatly increase homing velocity.
                projectile.extraUpdates = projectile.MogMod().defExtraUpdates + 1;

                // Home in on the target.
                Vector2 homeDirection = (destination - projectile.Center).SafeNormalize(Vector2.UnitY);
                projectile.velocity = (projectile.velocity * inertia + homeDirection * homingVelocity) / (inertia + 1f);
            }
            else
            {
                // Set amount of extra updates to default amount.
                projectile.extraUpdates = projectile.MogMod().defExtraUpdates;
            }
        }

        public static bool AnyMarkedNPCAlive()
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.active)
                    continue;

                if (!npc.TryGetGlobalNPC<MogModGlobalNPC>(out var globalNPC))
                    continue;

                if (globalNPC.markedByMarker)
                    return true;
            }

            return false;
        }

        // for magnet sphere like weapons
        public static void MagnetSphereHitscan(Projectile projectile, float distanceRequired, float homingVelocity, float projectileTimer, int maxTargets, int spawnedProjectile, double damageMult = 1D, bool attackMultiple = false)
        {
            // Only shoot once every N frames.
            projectile.localAI[1] += 1f;
            if (projectile.localAI[1] > projectileTimer)
            {
                projectile.localAI[1] = 0f;

                // Only search for targets if projectiles could be fired.
                float maxDistance = distanceRequired;
                bool homeIn = false;
                int[] targetArray = new int[maxTargets];
                int targetArrayIndex = 0;

                // once again ignore red line
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.CanBeChasedBy(projectile, false))
                    {
                        float extraDistance = (n.width / 2) + (n.height / 2);

                        bool canHit = true;
                        if (extraDistance < maxDistance)
                            canHit = Collision.CanHit(projectile.Center, 1, 1, n.Center, 1, 1);

                        if (projectile.WithinRange(n.Center, maxDistance + extraDistance) && canHit)
                        {
                            if (targetArrayIndex < maxTargets)
                            {
                                targetArray[targetArrayIndex] = n.whoAmI;
                                targetArrayIndex++;
                                homeIn = true;
                            }
                            else
                                break;
                        }
                    }
                }

                // If there is anything to actually shoot at, pick targets at random and fire.
                if (homeIn)
                {
                    int randomTarget = Main.rand.Next(targetArrayIndex);
                    randomTarget = targetArray[randomTarget];

                    projectile.localAI[1] = 0f;
                    Vector2 spawnPos = projectile.Center + projectile.velocity * 4f;
                    Vector2 velocity = Vector2.Normalize(Main.npc[randomTarget].Center - spawnPos) * homingVelocity;

                    if (attackMultiple)
                    {
                        for (int i = 0; i < targetArrayIndex; i++)
                        {
                            velocity = Vector2.Normalize(Main.npc[targetArray[i]].Center - spawnPos) * homingVelocity;

                            if (projectile.owner == Main.myPlayer)
                            {
                                int projectile2 = Projectile.NewProjectile(projectile.GetSource_FromThis(), spawnPos, velocity, spawnedProjectile, (int)(projectile.damage * damageMult), projectile.knockBack, projectile.owner, 0f, 0f);
                            }
                        }

                        return;
                    }

                    if (projectile.owner == Main.myPlayer)
                    {
                        int projectile2 = Projectile.NewProjectile(projectile.GetSource_FromThis(), spawnPos, velocity, spawnedProjectile, (int)(projectile.damage * damageMult), projectile.knockBack, projectile.owner, 0f, 0f);
                    }
                }
            }
        }
        public static int DamageSoftCap(double dmgInput, int cap)
        {
            // If the incoming damage is less than the cap, don't do anything.
            if (dmgInput < cap)
                return (int)dmgInput;

            // Ratio of how far over the cap you are.
            // This is a value from 1.0 upwards to theoretically infinity.
            double overpoweredRatio = dmgInput / cap;

            // Formula which reduces how "overpowered" you are to a reasonable level.
            double cappedRatio = Math.Pow(overpoweredRatio, 0.5) / 1.25 + 0.2;

            // Take the reduced ratio and multiply the cap by it to get the final capped damage.
            return (int)(cap * cappedRatio);
        }
        public static int DamageHardCap(double dmgInput, int cap)
        {
            // If the incoming damage is less than the cap, don't do anything.
            if (dmgInput < cap)
                return (int)dmgInput;
            return cap;
        }

        /// <summary>
        /// Call this function in the ai of your projectile so it can stick to enemies, also requires ModifyHitNPCSticky to be called in ModifyHitNPC
        /// </summary>
        /// <param name="projectile">The projectile you're adding sticky behaviour to</param>
        /// <param name="timeLeft">Number of seconds you want a projectile to cling to an NPC</param>
        public static void StickyNPCProjAI(this Projectile projectile, int timeLeft, bool findNewNPC = false)
        {
            if (projectile.ai[0] == 1f)
            {
                int seconds = timeLeft;
                bool killProj = false;
                bool spawnDust = false;

                //the projectile follows the NPC, even if it goes into blocks
                projectile.tileCollide = false;

                //timer for triggering hit effects
                projectile.localAI[0]++;
                if (projectile.localAI[0] % 30f == 0f)
                {
                    spawnDust = true;
                }

                //So AI knows what NPC it is sticking to
                int npcIndex = (int)projectile.ai[1];
                NPC npc = Main.npc[npcIndex];

                //Kill projectile after so many seconds or if the NPC it is stuck to no longer exists
                if (projectile.localAI[0] >= (float)(60 * seconds))
                {
                    killProj = true;
                }
                else if (!npcIndex.WithinBounds(Main.maxNPCs))
                {
                    killProj = true;
                }

                else if (npc.active && !npc.dontTakeDamage)
                {
                    //follow the NPC
                    projectile.Center = npc.Center - projectile.velocity * 2f;
                    projectile.gfxOffY = npc.gfxOffY;

                    //if attached to npc, trigger npc hit effects every half a second
                    if (spawnDust)
                    {
                        npc.HitEffect(0, 1.0);
                    }
                }
                else
                {
                    killProj = true;
                }

                //Kill the projectile or reset stats if needed
                if (killProj)
                {
                    if (findNewNPC)
                        projectile.ai[0] = 0f;
                    else
                        projectile.Kill();
                }
            }
        }
        /// <summary>
        /// Call this function in ModifyHitNPC to make your projectiles stick to enemies, needs StickyNPCProjAI to be called in the AI of the projectile
        /// </summary>
        /// <param name="projectile">The projectile you're giving sticky behaviour to</param>
        /// <param name="maxStick">How many projectiles of this type can stick to one enemy</param>
        public static void ModifyHitNPCSticky(this Projectile projectile, int maxStick)
        {
            Player player = Main.player[projectile.owner];
            Rectangle myRect = projectile.Hitbox;

            if (projectile.owner == Main.myPlayer)
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    //covers most edge cases like voodoo dolls
                    if (npc.active && !npc.dontTakeDamage &&
                        ((projectile.friendly && (!npc.friendly || (npc.type == NPCID.Guide && projectile.owner < Main.maxPlayers && player.killGuide) || (npc.type == NPCID.Clothier && projectile.owner < Main.maxPlayers && player.killClothier))) ||
                        (projectile.hostile && npc.friendly && !npc.dontTakeDamageFromHostiles)) && (projectile.owner < 0 || npc.immune[projectile.owner] == 0 || projectile.maxPenetrate == 1))
                    {
                        if (npc.noTileCollide || !projectile.ownerHitCheck)
                        {
                            bool stickingToNPC;
                            //Solar Crawltipede tail has special collision
                            if (npc.type == NPCID.SolarCrawltipedeTail)
                            {
                                Rectangle rect = npc.Hitbox;
                                int crawltipedeHitboxMod = 8;
                                rect.X -= crawltipedeHitboxMod;
                                rect.Y -= crawltipedeHitboxMod;
                                rect.Width += crawltipedeHitboxMod * 2;
                                rect.Height += crawltipedeHitboxMod * 2;
                                stickingToNPC = projectile.Colliding(myRect, rect);
                            }
                            else
                            {
                                stickingToNPC = projectile.Colliding(myRect, npc.Hitbox);
                            }
                            if (stickingToNPC)
                            {
                                //reflect projectile if the npc can reflect it (like Selenians)
                                if (npc.reflectsProjectiles && projectile.CanBeReflected())
                                {
                                    npc.ReflectProjectile(projectile);
                                    return;
                                }

                                //let the projectile know it is sticking and the npc it is sticking too
                                projectile.ai[0] = 1f;
                                projectile.ai[1] = (float)npcIndex;

                                //follow the NPC
                                projectile.velocity = (npc.Center - projectile.Center);

                                projectile.netUpdate = true;

                                //Count how many projectiles are attached, delete as necessary
                                Point[] array2 = new Point[maxStick];
                                int projCount = 0;
                                for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
                                {
                                    Projectile proj = Main.projectile[projIndex];
                                    if (projIndex != projectile.whoAmI && proj.active && proj.owner == Main.myPlayer && proj.type == projectile.type && proj.ai[0] == 1f && proj.ai[1] == (float)npcIndex)
                                    {
                                        array2[projCount++] = new Point(projIndex, proj.timeLeft);
                                        if (projCount >= array2.Length)
                                        {
                                            break;
                                        }
                                    }
                                }
                                if (projCount >= array2.Length)
                                {
                                    int stuckProjAmt = 0;
                                    for (int m = 1; m < array2.Length; m++)
                                    {
                                        if (array2[m].Y < array2[stuckProjAmt].Y)
                                        {
                                            stuckProjAmt = m;
                                        }
                                    }
                                    Main.projectile[array2[stuckProjAmt].X].Kill();
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void StickToTiles(this Projectile projectile, bool ignorePlatforms, bool stickToEverything)
        {
            try
            {
                int xLeft = (int)(projectile.position.X / 16f) - 1;
                int xRight = (int)((projectile.position.X + (float)projectile.width) / 16f) + 2;
                int yBottom = (int)(projectile.position.Y / 16f) - 1;
                int yTop = (int)((projectile.position.Y + (float)projectile.height) / 16f) + 2;
                if (xLeft < 0)
                {
                    xLeft = 0;
                }
                if (xRight > Main.maxTilesX)
                {
                    xRight = Main.maxTilesX;
                }
                if (yBottom < 0)
                {
                    yBottom = 0;
                }
                if (yTop > Main.maxTilesY)
                {
                    yTop = Main.maxTilesY;
                }
                for (int x = xLeft; x < xRight; x++)
                {
                    for (int y = yBottom; y < yTop; y++)
                    {
                        Tile tile = Main.tile[x, y];
                        bool platformCheck = true;
                        if (ignorePlatforms)
                            platformCheck = !TileID.Sets.Platforms[tile.TileType] && tile.TileType != TileID.PlanterBox;
                        bool tableCheck = false;
                        if (stickToEverything)
                            tableCheck = Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0;
                        if (tile != null && tile.HasUnactuatedTile && platformCheck && (Main.tileSolid[tile.TileType] || tableCheck))
                        {
                            Vector2 tileSize;
                            tileSize.X = (float)(x * 16);
                            tileSize.Y = (float)(y * 16);
                            if (projectile.position.X + (float)projectile.width - 4f > tileSize.X && projectile.position.X + 4f < tileSize.X + 16f && projectile.position.Y + (float)projectile.height - 4f > tileSize.Y && projectile.position.Y + 4f < tileSize.Y + 16f)
                            {
                                projectile.velocity.X = 0f;
                                projectile.velocity.Y = -0.2f;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
        /// <summary>
        /// Calculates a velocity that approximately predicts where some target will be in the future based on Euler's Method. This takes into account the projectile's max updates.
        /// </summary>
        /// <param name="startingPosition">The starting position from where the movement is calculated.</param>
        /// <param name="targetPosition">The position of the target to hit.</param>
        /// <param name="targetVelocity">The velocity of the target to hit.</param>
        /// <param name="shootSpeed">The speed of the predictive velocity.</param>
        /// <param name="projMaxUpdates">How many extra updates the resulting projectile will have.</param>
        /// <param name="iterations">The number of iterations to perform. The more iterations, the more precise the results are.</param>
        public static Vector2 CalculatePredictiveAimToTargetMaxUpdates(Vector2 startingPosition, Vector2 targetPosition, Vector2 targetVelocity, float shootSpeed, int projMaxUpdates, int iterations = 4)
        {
            float previousTimeToReachDestination = 0f;
            Vector2 currentTargetPosition = targetPosition;
            for (int i = 0; i < iterations; i++)
            {
                float timeToReachDestination = Vector2.Distance(startingPosition, currentTargetPosition) / shootSpeed / projMaxUpdates;
                currentTargetPosition += targetVelocity * (timeToReachDestination - previousTimeToReachDestination);
                previousTimeToReachDestination = timeToReachDestination;
            }
            return (currentTargetPosition - startingPosition).SafeNormalize(Vector2.UnitY) * shootSpeed;
        }
        /// <summary>
        /// Calculates a velocity that approximately predicts where some target will be in the future based on Euler's Method. This takes into account the projectile's max updates.
        /// </summary>
        /// <param name="startingPosition">The starting position from where the movement is calculated.</param>
        /// <param name="target">The target to hit.</param>
        /// <param name="shootSpeed">The speed of the predictive velocity.</param>
        /// <param name="projMaxUpdates">How many extra updates the resulting projectile will have.</param>
        /// <param name="iterations">The number of iterations to perform. The more iterations, the more precise the results are.</param>
        public static Vector2 CalculatePredictiveAimToTargetMaxUpdates(Vector2 startingPosition, Entity target, float shootSpeed, int projMaxUpdates, int iterations = 4)
        {
            return CalculatePredictiveAimToTargetMaxUpdates(startingPosition, target.Center, target.velocity, shootSpeed, projMaxUpdates, iterations);
        }
        public static Projectile FindProjectileByIdentity(int identity, int owner)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.identity == identity && proj.owner == owner)
                {
                    return proj;
                }
            }
            return null;
        }
        #endregion

        #region Hitboxes
        // expands hitbox
        public static void ExpandHitboxBy(this Projectile projectile, int width, int height)
        {
            projectile.position = projectile.Center;
            projectile.width = width;
            projectile.height = height;
            projectile.position -= projectile.Size * 0.5f;
        }
        public static void ExpandHitboxBy(this Projectile projectile, int newSize) => projectile.ExpandHitboxBy(newSize, newSize);
        public static void ExpandHitboxBy(this Projectile projectile, Vector2 newSize) => projectile.ExpandHitboxBy((int)newSize.X, (int)newSize.Y);
        public static void ExpandHitboxBy(this Projectile projectile, float expandRatio) => projectile.ExpandHitboxBy((int)(projectile.width * expandRatio), (int)(projectile.height * expandRatio));
        public static Color MulticolorLerp(float increment, params Color[] colors)
        {
            increment %= 0.999f;
            int currentColorIndex = (int)(increment * colors.Length);
            Color currentColor = colors[currentColorIndex];
            Color nextColor = colors[(currentColorIndex + 1) % colors.Length];
            return Color.Lerp(currentColor, nextColor, increment * colors.Length % 1f);
        }

        /// <summary>
        /// Determines if a typical hitbox rectangle is intersecting a circular hitbox.
        /// </summary>
        /// <param name="centerCheckPosition">The center of the circular hitbox.</param>
        /// <param name="radius">The radius of the circular hitbox.</param>
        /// <param name="targetHitbox">The hitbox of the target to check.</param>
        public static bool CircularHitboxCollision(Vector2 centerCheckPosition, float radius, Rectangle targetHitbox)
        {
            // If the center intersects the hitbox, return true immediately
            Rectangle center = new Rectangle((int)centerCheckPosition.X, (int)centerCheckPosition.Y, 1, 1);
            if (center.Intersects(targetHitbox))
                return true;

            float topLeftDistance = Vector2.Distance(centerCheckPosition, targetHitbox.TopLeft());
            float topRightDistance = Vector2.Distance(centerCheckPosition, targetHitbox.TopRight());
            float bottomLeftDistance = Vector2.Distance(centerCheckPosition, targetHitbox.BottomLeft());
            float bottomRightDistance = Vector2.Distance(centerCheckPosition, targetHitbox.BottomRight());

            float distanceToClosestPoint = topLeftDistance;
            if (topRightDistance < distanceToClosestPoint)
                distanceToClosestPoint = topRightDistance;
            if (bottomLeftDistance < distanceToClosestPoint)
                distanceToClosestPoint = bottomLeftDistance;
            if (bottomRightDistance < distanceToClosestPoint)
                distanceToClosestPoint = bottomRightDistance;

            return distanceToClosestPoint <= radius;
        }
        #endregion

        #region Projectile Afterimages / Netcode / Textures
        /// <summary>
        /// Draws a projectile as a series of afterimages. The first of these afterimages is centered on the center of the projectile's hitbox.<br />
        /// This function is guaranteed to draw the projectile itself, even if it has no afterimages and/or the Afterimages config option is turned off.
        /// </summary>
        /// <param name="proj">The projectile to be drawn.</param>
        /// <param name="mode">The type of afterimage drawing code to use. Vanilla Terraria has three options: 0, 1, and 2.</param>
        /// <param name="lightColor">The light color to use for the afterimages.</param>
        /// <param name="typeOneIncrement">If mode 1 is used, this controls the loop increment. Set it to more than 1 to skip afterimages.</param>
        /// <param name="texture">The texture to draw. Set to <b>null</b> to draw the projectile's own loaded texture.</param>
        /// <param name="drawCentered">If <b>false</b>, the afterimages will be centered on the projectile's position instead of its own center.</param>
        public static void DrawAfterimagesCentered(Projectile proj, int mode, Color lightColor, int typeOneIncrement = 1, Texture2D texture = null, bool drawCentered = true, float scale = 1f)
        {
            if (texture is null)
                texture = TextureAssets.Projectile[proj.type].Value;

            int frameHeight = texture.Height / Main.projFrames[proj.type];
            int frameY = frameHeight * proj.frame;
            float drawScale = proj.scale * scale;
            float rotation = proj.rotation;

            Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = rectangle.Size() / 2f;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (proj.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // If no afterimages are drawn due to an invalid mode being specified, ensure the projectile itself is drawn anyway.
            bool failedToDrawAfterimages = false;

            if (MogClientConfig.Instance.Afterimages)
            {
                Vector2 centerOffset = drawCentered ? proj.Size / 2f : Vector2.Zero;
                Color alphaColor = proj.GetAlpha(lightColor);
                switch (mode)
                {
                    // Standard afterimages. No customizable features other than total afterimage count.
                    // Type 0 afterimages linearly scale down from 100% to 0% opacity. Their color and lighting is equal to the main projectile's.
                    case 0:
                        for (int i = 0; i < proj.oldPos.Length; ++i)
                        {
                            Vector2 drawPos = proj.oldPos[i] + centerOffset - Main.screenPosition + new Vector2(0f, proj.gfxOffY);
                            // DO NOT REMOVE THESE "UNNECESSARY" FLOAT CASTS. THIS WILL BREAK THE AFTERIMAGES.
                            Color color = alphaColor * ((float)(proj.oldPos.Length - i) / (float)proj.oldPos.Length);
                            Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color, rotation, origin, drawScale, spriteEffects, 0f);
                        }
                        break;

                    // Paladin's Hammer style afterimages. Can be optionally spaced out further by using the typeOneDistanceMultiplier variable.
                    // Type 1 afterimages linearly scale down from 66% to 0% opacity. They otherwise do not differ from type 0.
                    case 1:
                        // Safety check: the loop must increment
                        int increment = Math.Max(1, typeOneIncrement);
                        Color drawColor = alphaColor;
                        int afterimageCount = ProjectileID.Sets.TrailCacheLength[proj.type];
                        float afterimageColorCount = (float)afterimageCount * 1.5f;
                        int k = 0;
                        while (k < afterimageCount)
                        {
                            Vector2 drawPos = proj.oldPos[k] + centerOffset - Main.screenPosition + new Vector2(0f, proj.gfxOffY);
                            // DO NOT REMOVE THESE "UNNECESSARY" FLOAT CASTS EITHER.
                            if (k > 0)
                            {
                                float colorMult = (float)(afterimageCount - k);
                                drawColor *= colorMult / afterimageColorCount;
                            }
                            Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), drawColor, rotation, origin, drawScale, spriteEffects, 0f);
                            k += increment;
                        }
                        break;

                    // Standard afterimages with rotation. No customizable features other than total afterimage count.
                    // Type 2 afterimages linearly scale down from 100% to 0% opacity. Their color and lighting is equal to the main projectile's.
                    case 2:
                        for (int i = 0; i < proj.oldPos.Length; ++i)
                        {
                            float afterimageRot = proj.oldRot[i];
                            SpriteEffects sfxForThisAfterimage = proj.oldSpriteDirection[i] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                            Vector2 drawPos = proj.oldPos[i] + centerOffset - Main.screenPosition + new Vector2(0f, proj.gfxOffY);
                            // DO NOT REMOVE THESE "UNNECESSARY" FLOAT CASTS. THIS WILL BREAK THE AFTERIMAGES.
                            Color color = alphaColor * ((float)(proj.oldPos.Length - i) / (float)proj.oldPos.Length);
                            Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color, afterimageRot, origin, drawScale, sfxForThisAfterimage, 0f);
                        }
                        break;

                    default:
                        failedToDrawAfterimages = true;
                        break;
                }
            }
        }        // Used for bullets. This lets you draw afterimages while keeping the hitbox at the front of the projectile.
        // This supports type 0 and type 2 afterimages. Vanilla bullets never have type 2 afterimages.
        public static void DrawAfterimagesFromEdge(Projectile proj, int mode, Color lightColor, Texture2D texture = null)
        {
            if (texture is null)
                texture = TextureAssets.Projectile[proj.type].Value;

            int frameHeight = texture.Height / Main.projFrames[proj.type];
            int frameY = frameHeight * proj.frame;
            float scale = proj.scale;
            float rotation = proj.rotation;

            Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (proj.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, proj.height * 0.5f);

            switch (mode)
            {
                default: // If you specify an afterimage mode other than 0 or 2, you get nothing.
                    return;

                // Standard afterimages. No customizable features other than total afterimage count.
                // Type 0 afterimages linearly scale down from 100% to 0% opacity. Their color and lighting is equal to the main projectile's.
                case 0:
                    for (int i = 0; i < proj.oldPos.Length; ++i)
                    {
                        Vector2 drawPos = proj.oldPos[i] + drawOrigin - Main.screenPosition + new Vector2(0f, proj.gfxOffY);
                        // DO NOT REMOVE THESE "UNNECESSARY" FLOAT CASTS. THIS WILL BREAK THE AFTERIMAGES.
                        Color color = proj.GetAlpha(lightColor) * ((float)(proj.oldPos.Length - i) / (float)proj.oldPos.Length);
                        Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color, rotation, drawOrigin, scale, spriteEffects, 0f);
                    }
                    return;

                // Standard afterimages with rotation. No customizable features other than total afterimage count.
                // Type 2 afterimages linearly scale down from 100% to 0% opacity. Their color and lighting is equal to the main projectile's.
                case 2:
                    for (int i = 0; i < proj.oldPos.Length; ++i)
                    {
                        float afterimageRot = proj.oldRot[i];
                        SpriteEffects sfxForThisAfterimage = proj.oldSpriteDirection[i] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                        Vector2 drawPos = proj.oldPos[i] + drawOrigin - Main.screenPosition + new Vector2(0f, proj.gfxOffY);
                        // DO NOT REMOVE THESE "UNNECESSARY" FLOAT CASTS. THIS WILL BREAK THE AFTERIMAGES.
                        Color color = proj.GetAlpha(lightColor) * ((float)(proj.oldPos.Length - i) / (float)proj.oldPos.Length);
                        Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color, afterimageRot, drawOrigin, scale, sfxForThisAfterimage, 0f);
                    }
                    return;
            }
        }
        public static void DrawStarTrail(this Projectile projectile, Color outer, Color inner, float auraHeight = 10f)
        {
            Texture2D aura = Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/StarTrail").Value;
            Vector2 offsets = new Vector2(0f, projectile.gfxOffY) - Main.screenPosition;
            Rectangle auraRec = aura.Frame();
            float auraRotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Vector2 auraOrigin = new Vector2(auraRec.Width / 2f, auraHeight);

            // Outer trail
            Vector2 drawStartOuter = offsets + projectile.Center + projectile.velocity;
            Vector2 spinPoint = -Vector2.UnitY * auraHeight;
            float time = Main.player[projectile.owner].miscCounter % 216000f / 60f;
            Color outerColor = outer * 0.2f;
            outerColor.A = 0;
            float rotation = MathHelper.TwoPi * time;
            for (int o = 0; o < 6; o += 2)
            {
                Vector2 spinStart = drawStartOuter + spinPoint.RotatedBy(rotation - MathHelper.Pi * o / 3f);
                float scaleMultOuter = 1.5f - o * 0.1f;
                Main.EntitySpriteDraw(aura, spinStart, auraRec, outerColor, auraRotation, auraOrigin, scaleMultOuter, SpriteEffects.None, 0);
            }

            // Inner trail
            Vector2 drawStartInner = offsets + projectile.Center - projectile.velocity * 0.5f;
            Color innerColor = inner * 0.5f;
            innerColor.A = 0;
            for (float i = 0f; i < 1f; i += 0.5f)
            {
                float scaleMult = time % 0.5f / 0.5f;
                scaleMult = (scaleMult + i) % 1f;
                float colorMult = scaleMult * 2f;
                if (colorMult > 1f)
                    colorMult = 2f - colorMult;

                Main.EntitySpriteDraw(aura, drawStartInner, auraRec, innerColor * colorMult, auraRotation, auraOrigin, 0.3f + scaleMult * 0.5f, SpriteEffects.None, 0);
            }
        }
        public static void ForceNetUpdate(this Projectile proj, bool ignoreCurrentNetSpam = true)
        {
            proj.netUpdate = true;
            if (proj.netSpam >= 10 || ignoreCurrentNetSpam)
                proj.netSpam = 0;
        }
        #endregion
    }
}
