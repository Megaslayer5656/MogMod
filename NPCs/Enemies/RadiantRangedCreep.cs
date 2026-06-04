using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Banners;
using MogMod.Items.Weapons.Magic.SorceryStaves;
using MogMod.Projectiles.EnemyProjectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace MogMod.NPCs.Enemies
{
    public class RadiantRangedCreep : ModNPC
    {
        #region Setup
        private Player Target
        {
            get
            {
                if (NPC.HasValidTarget)
                {
                    return Main.player[NPC.target];
                }
                return null;
            }
        }
        private float grounded_counter
        {
            get
            {
                return NPC.ai[1];
            }
            set
            {
                NPC.ai[1] = value;
            }
        }
        private float target_walkMaxSpeed = 1.6f;
        private float target_walkAcceleration = 0.12f;
        public bool Shooting = false;
        public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 14;
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 40;

            NPC.damage = 8;
            NPC.defense = 0;
            NPC.lifeMax = 40;
            NPC.knockBackResist = 1f;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.value = Item.buyPrice(copper: 80);

            NPC.aiStyle = -1;
            AIType = -1;

            Banner = NPC.type;
            BannerItem = ModContent.ItemType<RadiantRangedCreepBanner>();
        }
        #endregion

        #region Bestiary && Loot
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.MogMod.Bestiary.RadiantRangedCreep")
            ]);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ManaEssence>(), 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CreepBlood>(), 1, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GlintstoneStaff>(), 20, 1, 1));
        }
        #endregion

        #region Spawning && AI
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !spawnInfo.Player.ZoneOverworldHeight)
                return 0f;
            if (spawnInfo.PlayerFloorX > Main.maxTilesX * 0.333f && spawnInfo.PlayerFloorX < Main.maxTilesX - Main.maxTilesX * 0.333f)
                return SpawnCondition.OverworldDaySlime.Chance * 0.1f;
            return SpawnCondition.OverworldDaySlime.Chance * 0.2f;
        }
        public override void AI()
        {
            NPC.TargetClosest(true);
            if (NPC.Distance(Target.Center) < 400f)
                Shoot();
            else
            {
                DoTargetAI();
                Shooting = false;
            }
        }
        private void DoTargetAI()
        {
            if (NPC.ai[0] >= 60f && NPC.ai[0] < 90f)
                NPC.ai[0] = 60f;
            if (NPC.velocity.Y == 0)
                grounded_counter++;
            else
                grounded_counter = 0;
            bool targetToLeft = Target.Center.X < NPC.Center.X;
            int mult = targetToLeft ? -1 : 1;
            NPC.velocity.X += target_walkAcceleration * mult;
            if (Math.Abs(NPC.velocity.X) > target_walkMaxSpeed)
                NPC.velocity.X = target_walkMaxSpeed * mult;
            NPC.direction = NPC.velocity.X < 0f ? -1 : 1;
            NPC.spriteDirection = NPC.direction;
            if (grounded_counter > 10 && (HoleBelow() || (NPC.collideX && NPC.position.X == NPC.oldPosition.X)))
                NPC.velocity.Y = -6f;
        }
        private void Shoot()
        {
            var entitySource = NPC.GetSource_FromAI();
            NPC.ai[0]++;
            NPC.velocity.X *= 0.9f;
            if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                NPC.velocity.X = 0f;
            if (NPC.ai[0] >= 60f)
            {
                if (NPC.ai[0] < 90f)
                    Shooting = true;
                if (NPC.ai[0] == 90f)
                {
                    Vector2 vecToPlayer = NPC.DirectionTo(Target.Center);
                    Vector2 projVelocity = vecToPlayer * 3f;
                    int type = ModContent.ProjectileType<RadiantRangedCreepProj>();
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int damage = Main.masterMode ? 7 : Main.expertMode ? 8 : 10;
                        int projectile = Projectile.NewProjectile(NPC.GetSource_FromAI(),
                            NPC.Center + projVelocity.SafeNormalize(Vector2.Zero) * 10f,
                            projVelocity,
                            type,
                            damage,
                            0f,
                            Main.myPlayer);
                        NPC.netUpdate = true;
                    }
                }
                if (NPC.ai[0] == 100f)
                {
                    Shooting = false;
                    NPC.ai[0] = 10f;
                }
            }
        }
        private bool HoleBelow()
        {
            //width of npc in tiles
            int tileWidth = 2;
            int tileX = (int)(NPC.Center.X / 16f) - tileWidth;
            if (NPC.velocity.X > 0) //if moving right
            {
                tileX += tileWidth;
            }
            int tileY = (int)((NPC.position.Y + NPC.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + tileWidth; x++)
                {
                    if (Main.tile[x, y].HasTile)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public override void FindFrame(int frameHeight)
        {
            // attack animation
            if (Shooting || NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter += 1.0;
                // set frameCounter to 0 so the animation plays properly
                if (NPC.frameCounter > 7.0)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                }
                if (NPC.frame.Y < frameHeight * 9)
                    NPC.frame.Y = frameHeight * 9;
                if (NPC.frame.Y > frameHeight * 13)
                    NPC.frame.Y = frameHeight * 9;
            }
            // jumping animation
            else if (NPC.velocity.Y != 0.0)
                NPC.frame.Y = frameHeight * 8;
            // walking animation
            else
            {
                NPC.frameCounter += (double)Math.Abs(NPC.velocity.X);
                // reset the frame counter
                if (NPC.frameCounter > 7.0)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                }
                // if the npc is in the air, dont change direction
                if (NPC.velocity.Y == 0f)
                {
                    if (NPC.direction == 1)
                        NPC.spriteDirection = 1;
                    if (NPC.direction == -1)
                        NPC.spriteDirection = -1;
                }
                else
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = frameHeight;
                    return;
                }
                if (NPC.velocity.X == 0f)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = 0;
                }
                else
                {
                    if (NPC.frame.Y > frameHeight * 7)
                        NPC.frame.Y = 0;
                }
            }
        }
        #endregion
    }
}