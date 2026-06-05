using MogMod.Items.Other;
using MogMod.Items.Placeable.Banners;
using MogMod.Items.Weapons.Magic.SorceryStaves;
using MogMod.Projectiles.EnemyProjectiles;
using System;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using ReLogic.Text;

namespace MogMod.NPCs.Enemies
{
    public class TwilightDrover : ModNPC
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
        public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 11;
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 46;

            NPC.damage = 8;
            NPC.defense = 0;
            NPC.lifeMax = 40;
            NPC.knockBackResist = 1f;
            NPC.alpha = 50;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.value = Item.buyPrice(silver: 1, copper: 80);

            NPC.aiStyle = -1;
            AIType = -1;

            Banner = NPC.type;
            BannerItem = ModContent.ItemType<TwilightDroverBanner>();
        }
        #endregion

        #region Bestiary && Loot
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.MogMod.Bestiary.TwilightDrover")
            ]);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ManaEssence>(), 8, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpiritShard>(), 1, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AstrologersStaff>(), 20, 1, 1));
        }
        #endregion

        #region Spawning && AI
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !spawnInfo.Player.ZoneOverworldHeight || !Condition.DownedEowOrBoc.IsMet())
                return 0f;

            return SpawnCondition.OverworldNightMonster.Chance * .1f;
        }
        public override void AI()
        {
            NPC.TargetClosest(true);
            if (NPC.Distance(Target.Center) < 600f)
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
                        Projectile bolt = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(),
                            NPC.Center + projVelocity.SafeNormalize(Vector2.Zero) * 10f,
                            projVelocity,
                            type,
                            damage,
                            0f,
                            Main.myPlayer);
                        bolt.tileCollide = false;
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
            if (Shooting || NPC.IsABestiaryIconDummy)
            {
                if (NPC.frame.Y < frameHeight * 10)
                    NPC.frame.Y = frameHeight * 10;

                NPC.frameCounter += 1.0;

                if (NPC.frame.Y < frameHeight * 10)
                    NPC.frame.Y = frameHeight * 10;
                if (NPC.frame.Y > frameHeight * 10)
                    NPC.frame.Y = frameHeight * 10;
            }
            else if (NPC.velocity.Y != 0.0)
                NPC.frame.Y = 0;
            else
            {
                if (NPC.frame.Y >= frameHeight * 10)
                    NPC.frame.Y = frameHeight;

                NPC.frameCounter += (double)Math.Abs(NPC.velocity.X);
                if (NPC.frameCounter > 8.0)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y < frameHeight)
                        NPC.frame.Y = frameHeight;

                    if (NPC.frame.Y > frameHeight * 10)
                        NPC.frame.Y = frameHeight;
                }
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
                    NPC.frame.Y = frameHeight;
                }
                else
                {
                    if (NPC.frame.Y > frameHeight * 10)
                        NPC.frame.Y = frameHeight;
                }
            }
        }
        #endregion
    }
}