using Microsoft.Xna.Framework;
using MogMod.Items.Ammo;
using MogMod.Items.Placeable.Banners;
using MogMod.Items.Weapons.Ranged;
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
    public class Scav : ModNPC
    {
        #region Setup
        private Player Target
        {
            get
            {
                if (NPC.HasValidTarget)
                    return Main.player[NPC.target];
                return null;
            }
        }
        private float grounded_counter
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        private float target_walkMaxSpeed = 1.6f;
        private float target_walkAcceleration = 0.12f;
        public bool Shooting = false;
        public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 30;
        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 48;

            NPC.damage = 16;
            NPC.defense = 4;
            NPC.lifeMax = 110;
            NPC.knockBackResist = .4f;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.value = Item.buyPrice(silver: 12);

            NPC.aiStyle = -1;
            AIType = -1;

            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ScavBanner>();
        }
        #endregion

        #region Bestiary && Loot
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.MogMod.Bestiary.Scav")
            ]);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // TODO: add/replace an item with something better
            npcLoot.Add(ItemDropRule.Common(ItemID.IllegalGunParts, 10, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MosinLPS>(), 1, 4, 12));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Mosin>(), 20, 1, 1));
        }
        #endregion

        #region Spawning && AI
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !NPC.downedBoss1)
                return 0f;
            if (spawnInfo.PlayerFloorX > Main.maxTilesX * 0.333f && spawnInfo.PlayerFloorX < Main.maxTilesX - Main.maxTilesX * 0.333f)
                return 0f;
            if (Main.dayTime)
                return SpawnCondition.OverworldDaySlime.Chance * .02f;
            return SpawnCondition.OverworldNightMonster.Chance * .08f;
        }
        public override void AI()
        {
            target_walkMaxSpeed = Shooting ? .8f : 1.6f;
            NPC.TargetClosest(true);
            DoTargetAI();
            if (NPC.Distance(Target.Center) < 450f)
                Shoot();
            else
                Shooting = false;
        }
        private void DoTargetAI()
        {
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
                NPC.velocity.Y = -5f;
        }
        private void Shoot()
        {
            var entitySource = NPC.GetSource_FromAI();
            Shooting = true;
            NPC.ai[0]++;
            if (NPC.ai[0] == 70f)
            {
                Vector2 vecToPlayer = NPC.DirectionTo(Target.Center);
                Vector2 projVelocity = vecToPlayer * 8f;
                int type = ProjectileID.BulletDeadeye;
                SoundEngine.PlaySound(SoundID.Item36, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int damage = Main.masterMode ? 10 : Main.expertMode ? 12 : 16;
                    int bulletAmt = 6;
                    for (int index = 0; index < bulletAmt; ++index)
                    {
                        Vector2 Center = NPC.Center + projVelocity.SafeNormalize(Vector2.Zero) * 10f;
                        float SpeedX = projVelocity.X + (float)Main.rand.Next(-25, 26) * 0.05f;
                        float SpeedY = projVelocity.Y + (float)Main.rand.Next(-25, 26) * 0.05f;
                        int projectile = Projectile.NewProjectile(NPC.GetSource_FromAI(), Center.X, Center.Y, SpeedX, SpeedY, type, damage, 0f, Main.myPlayer);
                    }
                    NPC.netUpdate = true;
                }
            }
            if (NPC.ai[0] == 80f)
                NPC.ai[0] = 00f;
        }
        private bool HoleBelow()
        {
            //width of npc in tiles
            int tileWidth = 2;
            int tileX = (int)(NPC.Center.X / 16f) - tileWidth;
            if (NPC.velocity.X > 0) //if moving right
                tileX += tileWidth;
            int tileY = (int)((NPC.position.Y + NPC.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
                for (int x = tileX; x < tileX + tileWidth; x++)
                    if (Main.tile[x, y].HasTile)
                        return false;
            return true;
        }
        public override void FindFrame(int frameHeight)
        {
            // jumping animation
            if (NPC.velocity.Y != 0.0)
                NPC.frame.Y = frameHeight;
            // walking animation
            else
            {
                NPC.frameCounter += (double)Math.Abs(NPC.velocity.X);
                if (NPC.IsABestiaryIconDummy)
                    NPC.frameCounter += 1D;
                // reset the frame counter
                if (NPC.frameCounter > 14.0)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y += frameHeight;
                }
                if (NPC.velocity.Y == 0f)
                {
                    if (NPC.direction == 1)
                        NPC.spriteDirection = 1;
                    if (NPC.direction == -1)
                        NPC.spriteDirection = -1;
                }
                if (Shooting || NPC.IsABestiaryIconDummy)
                {
                    if (NPC.frame.Y < frameHeight * 16)
                        NPC.frame.Y = frameHeight * 16;
                    if (NPC.frame.Y > frameHeight * 29)
                        NPC.frame.Y = frameHeight * 16;
                }
                else
                {
                    if (NPC.frame.Y < frameHeight * 2)
                        NPC.frame.Y = frameHeight * 2;
                    if (NPC.frame.Y > frameHeight * 14)
                        NPC.frame.Y = frameHeight * 2;
                }
            }
        }
        #endregion
    }
}