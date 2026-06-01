using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Banners;
using MogMod.NPCs.ProjectileEnemies;
using MogMod.Projectiles.EnemyProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.NPCs.Enemies
{
    public class WarlockGolem : ModNPC
    {
        #region Setup
        public float size = 0f;
        public bool canExplode = false;
        public bool exploding = false;
        private float target_walkMaxSpeed = 1.6f;
        private float target_walkAcceleration = 0.12f;
        private float grounded_counter
        {
            get
            {
                return NPC.ai[2];
            }
            set
            {
                NPC.ai[2] = value;
            }
        }
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
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 11;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.6f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }
        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 140;

            NPC.npcSlots = 3f;
            NPC.aiStyle = -1;

            NPC.damage = Main.hardMode ? 124 : 42;
            NPC.defense = Main.hardMode ? 40 : 22;
            NPC.lifeMax = Main.hardMode ? 1500 : 570;
            NPC.knockBackResist = Main.hardMode ? .02f : .05f;

            NPC.knockBackResist = 0.05f;
            NPC.lavaImmune = true;
            AIType = -1;
            NPC.value = Item.buyPrice(gold: 2);
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath43;
            NPC.rarity = 2;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<WarlockGolemBanner>();
            ItemID.Sets.KillsToBanner[BannerItem] = 25; // Custom kill count required for banner drop and bestiary unlock. Omit this line for the default 50 kill count.
        }
        #endregion

        #region Bestiary && Loot
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
				new FlavorTextBestiaryInfoElement("Mods.MogMod.Bestiary.WarlockGolem")
            ]);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            LeadingConditionRule postEvil = npcLoot.DefineConditionalDropSet(DropHelper.PostEvil());
            LeadingConditionRule postOneMech = npcLoot.DefineConditionalDropSet(DropHelper.PostOneMech());
            npcLoot.Add(ItemDropRule.Common(ItemID.Obsidian, 1, 12, 18));
            postEvil.Add(ItemID.Hellstone, 1, 12, 18);
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<ScorchedCore>(), 1, 1, 1));
            postOneMech.Add(ModContent.ItemType<HellfireEssence>(), 1, 1, 1);
        }
        #endregion

        #region Spawning && AI
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCorrupt ||
                spawnInfo.Player.ZoneCrimson ||
                spawnInfo.Player.ZoneOldOneArmy ||
                spawnInfo.Player.ZoneSkyHeight ||
                spawnInfo.PlayerSafe ||
                spawnInfo.Player.ZoneDesert ||
                spawnInfo.Player.ZoneOverworldHeight ||
                !spawnInfo.Player.ZoneUnderworldHeight ||
                Main.eclipse ||
                Main.snowMoon ||
                Main.pumpkinMoon ||
                Main.invasionType != InvasionID.None)
                return 0f;
            // Keep this as a separate if check, because it's a loop and we don't want to be checking it constantly.
            if (NPC.AnyNPCs(NPC.type))
                return 0f;
            return 0.04f;
        }
        public override void AI()
        {
            if (!NPC.HasValidTarget)
                NPC.TargetClosest(false);
            else
                DoTargetAI();
            if (NPC.velocity.Y == 0)
                grounded_counter++;
            else
                grounded_counter = 0;
            if (canExplode)
                Explode();
            if (!exploding && Condition.DownedMechBossAny.IsMet())
                Fireball();
        }
        private void DoTargetAI()
        {
            // code taken from calamity mods Atlas enemy
            bool targetToLeft = Target.Center.X < NPC.Center.X;
            int mult = targetToLeft ? -1 : 1;
            NPC.velocity.X += target_walkAcceleration * mult;
            if (Math.Abs(NPC.velocity.X) > target_walkMaxSpeed)
                NPC.velocity.X = target_walkMaxSpeed * mult;
            //based on velocity, as he a big hunk and he can't walk backwards whilst facing target
            NPC.direction = NPC.velocity.X < 0f ? -1 : 1;
            NPC.spriteDirection = NPC.direction;

            //if have been on ground for at least 1.5 seonds, and are hitting wall or there is a hole
            if (grounded_counter > 90 && (HoleBelow() || (NPC.collideX && NPC.position.X == NPC.oldPosition.X)))
                NPC.velocity.Y = -10f;
            Vector2 distance = NPC.Center - Target.Center;
            if (Math.Abs(distance.X) < 200 && Math.Abs(distance.Y) < 200)
                canExplode = true;
        }
        private void Explode()
        {
            exploding = true;
            NPC.ai[1] += 1f;
            if (NPC.ai[1] >= 0f)
            {
                NPC.velocity.X = 0f;
                if (NPC.ai[1] == 0f)
                    SoundEngine.PlaySound(SoundID.Zombie91, NPC.Center);
                size = (NPC.ai[1] * 5f) + 100f;
                for (int i = 0; i < 50; i++)
                {
                    Vector2 randomOffset = Main.rand.NextVector2Circular(size / 2.1f, size / 2.1f);
                    Dust d = Dust.NewDustPerfect(NPC.Center + randomOffset, DustID.Flare, NPC.DirectionFrom(NPC.Center + NPC.velocity + randomOffset) * Main.rand.NextFloat(3f, 5f));
                    d.fadeIn = .15f;
                    d.scale = .75f;
                    d.noGravity = true;
                }
                for (int i = 0; i < 70; i++)
                {
                    Vector2 randPos = Main.rand.NextVector2CircularEdge(size / 2f, size / 2f);
                    Dust telegraphDust = Dust.NewDustPerfect(NPC.Center + randPos, DustID.CopperCoin, NPC.DirectionFrom(NPC.Center + NPC.velocity + randPos) * Main.rand.NextFloat(5f, 7f), 0, default, 1.5f);
                    telegraphDust.noGravity = true;
                }
                if (NPC.ai[1] >= 90f)
                {
                    int type = ModContent.ProjectileType<WarlockBoom>();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int damage = Main.masterMode ? 16 : Main.expertMode ? 18 : 20;
                        int projectile = Projectile.NewProjectile(NPC.GetSource_FromAI(),
                            NPC.Center,
                            Vector2.Zero,
                            type,
                            damage,
                            6f,
                            Main.myPlayer);
                        NPC.netUpdate = true;
                    }
                    NPC.ai[1] = -60f;
                    exploding = false;
                    canExplode = false;
                }
            }
        }
        private void Fireball()
        {
            var entitySource = NPC.GetSource_FromAI();
            NPC.ai[0]++;
            if (NPC.ai[0] == 60f)
            {
                NPC fireball = NPC.NewNPCDirect(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WarlockFireball>(), NPC.whoAmI);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, number: fireball.whoAmI);
                NPC.ai[0] = -120f;
            }
        }
        private bool HoleBelow()
        {
            //width of npc in tiles
            int tileWidth = 4;
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
#endregion

        #region Frames && Hit Effects
        public override void FindFrame(int frameHeight)
        {
            if (!exploding)
                NPC.frameCounter += 0.1f;
            NPC.frameCounter %= Main.npcFrameCount[Type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SolarFlare, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SolarFlare, hit.HitDirection, -1f, 0, default, 2f);
                }
            }
        }
        #endregion
    }
}