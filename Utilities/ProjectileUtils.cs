using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Config;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

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
        #endregion

        #region Projectile Spawning
        // summons projectiles from the sky
        public static Projectile ProjectileRain(IEntitySource source, Vector2 targetPos, float xLimit, float xVariance, float yLimitLower, float yLimitUpper, float projSpeed, int projType, int damage, float knockback, int owner)
        {
            float x = targetPos.X + Main.rand.NextFloat(-xLimit, xLimit);
            float y = targetPos.Y - Main.rand.NextFloat(yLimitLower, yLimitUpper);
            Vector2 spawnPosition = new Vector2(x, y);
            Vector2 velocity = targetPos - spawnPosition;
            velocity.X += Main.rand.NextFloat(-xVariance, xVariance);
            float speed = projSpeed;
            float targetDist = velocity.Length();
            targetDist = speed / targetDist;
            velocity.X *= targetDist;
            velocity.Y *= targetDist;
            return Projectile.NewProjectileDirect(source, spawnPosition, velocity, projType, damage, knockback, owner);
        }

        // summons lots of projectiles
        public static Projectile ProjectileBarrage(IEntitySource source, Vector2 originVec, Vector2 targetPos, bool fromRight, float xOffsetMin, float xOffsetMax, float yOffsetMin, float yOffsetMax, float projSpeed, int projType, int damage, float knockback, int owner, bool clamped = false, float inaccuracyOffset = 5f)
        {
            float xPos = originVec.X + Main.rand.NextFloat(xOffsetMin, xOffsetMax) * fromRight.ToDirectionInt();
            float yPos = originVec.Y + Main.rand.NextFloat(yOffsetMin, yOffsetMax) * Main.rand.NextBool().ToDirectionInt();
            Vector2 spawnPosition = new Vector2(xPos, yPos);
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
            return Projectile.NewProjectileDirect(source, spawnPosition, velocity, projType, damage, knockback, owner);
        }

        // homing code (DOES WORK ! ! ! THANK YOU CALAMITY)
        public static void HomeInOnNPC(Projectile projectile, bool ignoreTiles, float distanceRequired, float homingVelocity, float inertia)
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

        #region Projectile Afterimages
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
        public static void DrawAfterimagesCentered(Projectile proj, int mode, Color lightColor, int typeOneIncrement = 1, Texture2D texture = null, bool drawCentered = true)
        {
            if (texture is null)
                texture = TextureAssets.Projectile[proj.type].Value;

            int frameHeight = texture.Height / Main.projFrames[proj.type];
            int frameY = frameHeight * proj.frame;
            float scale = proj.scale;
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
                            Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color, rotation, origin, scale, spriteEffects, 0f);
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
                            Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), drawColor, rotation, origin, scale, spriteEffects, 0f);
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
                            Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color, afterimageRot, origin, scale, sfxForThisAfterimage, 0f);
                        }
                        break;

                    default:
                        failedToDrawAfterimages = true;
                        break;
                }
            }
        }
        #endregion
    }
}
