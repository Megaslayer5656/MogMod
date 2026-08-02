using Microsoft.Xna.Framework;
using MogMod.Common.Packets;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Utilities
{
    public static partial class MogModUtils
    {
        #region Minion Homing
        // copied from calamity mod
        public static float Modulo(this float dividend, float divisor)
        {
            return dividend - (float)Math.Floor(dividend / divisor) * divisor;
        }
        // angular distance between two vectors
        public static float AngleBetween(this Vector2 v1, Vector2 v2) => (float)Math.Acos(Vector2.Dot(v1.SafeNormalize(Vector2.Zero), v2.SafeNormalize(Vector2.Zero)));
        public static float AngleBetween(this float angle, float otherAngle) => ((otherAngle - angle) + MathHelper.Pi).Modulo(MathHelper.TwoPi) - MathHelper.Pi;

        // calamity mod minion homing code
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
        internal const int TinyHealthThreshold = 5;
        internal const int TinyDamageThreshold = 5;
        internal const int NoContactDamageHealthThreshold = 3000;
        /// <summary>
        /// Syncs position and velocity from a client to the server. This is to be used in contexts where these things are reliant on client-side information, such as <see cref="Main.MouseWorld"/>.
        /// </summary>
        /// <param name="npc"></param>
        public static void SyncMotionToServer(this NPC npc)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return;

            SyncNPCMotionDataToServerPacket.Send(npc);
        }
        public static void SyncNPCPosAndRotOnly(this NPC npc)
        {
            SyncNPCPosAndRotOnlyPacket.Send(npc);
        }
        public static bool IsAnEnemy(this NPC npc, bool allowStatues = true, bool checkDead = true, bool checkDamage = true)
        {
            // Null, inactive, town NPCs, and friendlies are right out.
            if (npc is null || (!npc.active && (!checkDead || npc.life > 0)) || npc.townNPC || npc.friendly)
                return false;

            // Unless allowed, statue spawns don't count for rage.
            if (!allowStatues && npc.SpawnedFromStatue)
                return false;

            // "Non-enemies" (e.g. butterflies or projectile enemies) with near zero max health,
            // or anything but the strongest enemies with no contact damage (e.g. Celestial Pillars, Providence)
            // do not generate rage.
            if (npc.lifeMax <= TinyHealthThreshold || ((npc.defDamage <= TinyDamageThreshold && checkDamage) && npc.lifeMax <= NoContactDamageHealthThreshold))
                return false;

            // Exclude NPCs that specified to not be counted as enemy
            // This includes: TargetDummy, SuperDummy by Default
            if (npc.type == NPCID.TargetDummy)
                return false;

            // Anything else is considered a valid enemy target.
            return true;
        }
        /// <summary>
        /// Spawns a <see cref="CombatText"/> indicating the amount of damage manually dealt to the NPC, such as from self-damage. Automatically syncs it in multiplayer.
        /// </summary>
        public static void DamageEffect(this NPC npc, int damageAmount, Color color, bool dramatic = false)
        {
            Rectangle r = new((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
            Color textColor = color;
            int num = dramatic ? 1 : 0;
            if (Main.dedServ)
                NetMessage.SendData(MessageID.CombatTextInt, -1, -1, null, (int)textColor.PackedValue, r.Center.X, r.Center.Y, damageAmount); // TODO: find a way to make this dramatic
            else
                CombatText.NewText(r, textColor, damageAmount, dramatic);
        }
        /// <summary>
        /// Spawns a <see cref="CombatText"/> set to a custom text. Automatically syncs it in multiplayer.
        /// </summary>
        public static void TextEffect(NetworkText text, Rectangle rectangle, Color color, bool dramatic = false)
        {
            Color textColor = color;
            int num = dramatic ? 1 : 0;
            if (Main.dedServ)
                NetMessage.SendData(MessageID.CombatTextString, -1, -1, text, (int)textColor.PackedValue, rectangle.Center.X, rectangle.Center.Y); // TODO: find a way to make this dramatic
            else
                CombatText.NewText(rectangle, textColor, text.ToString(), dramatic);
        }
        /// <summary>
        /// Check if an NPC can be moved
        /// </summary>
        /// <param name="target">The NPC attacked.</param>
        /// <returns>Whether or not the NPC can be moved around.</returns>
        public static bool CanBeMoved(this NPC target, bool ignoreKBImmune = false)
        {
            // Ideally we can replace [!CalamityPlayer.areThereAnyDamnBosses] with a check for problematic boss minions so that you can knock back regular ones in bossfights.
            bool isAPillar = target.type == NPCID.LunarTowerSolar || target.type == NPCID.LunarTowerVortex || target.type == NPCID.LunarTowerNebula || target.type == NPCID.LunarTowerStardust;
            if (!isAPillar && !target.boss && target.IsAnEnemy(true, true, false) && (ignoreKBImmune || target.knockBackResist > 0))
                return true;
            return false;
        }
        /// <summary>
        /// Moves an NPC, usually used as custom knockback
        /// </summary>
        /// <param name="target">The NPC being moved.</param>
        /// <param name="ignoreKBImmune">Whether or not NPC's that normally have knockback immunity can be moved around.</param>
        public static void MoveNPC(this NPC target, Vector2 direction, float strength, bool heavyKnockback = false, Player attacker = null)
        {
            if (target.CanBeMoved())
            {
                Vector2 launchVel = direction.SafeNormalize(Vector2.UnitX) * strength;
                float playerKnockbackMult = 1;
                float knockbackMult = playerKnockbackMult * (heavyKnockback ? Math.Max(target.knockBackResist, 1) : target.knockBackResist); // Heavy knockback ignores knockback resist (but not knockback weakness)
                target.velocity = launchVel * knockbackMult;
                target.SyncMotionToServer();
            }
        }
        public static NPCShop AddWithCustomValue(this NPCShop shop, int itemType, int customValue, params Condition[] conditions)
        {
            var item = new Item(itemType)
            {
                shopCustomPrice = customValue
            };
            return shop.Add(item, conditions);
        }
        public static NPCShop AddWithCustomValue<T>(this NPCShop shop, int customValue, params Condition[] conditions) where T : ModItem
        {
            return shop.AddWithCustomValue(ItemType<T>(), customValue, conditions);
        }
    }
}
